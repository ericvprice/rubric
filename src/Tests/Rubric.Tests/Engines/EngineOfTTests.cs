using Microsoft.Extensions.Logging.Abstractions;
using Rubric.Engines.Implementation;
using Rubric.Rules;
using Rubric.Rulesets;
using Rubric.Tests.TestRules;

namespace Rubric.Tests.Engines;

public class EngineOfTTests
{
  [Fact]
  public void Applies()
  {
    var rule = new TestDefaultPreRule();
    var input = new TestInput();
    var engine = new RuleEngine<TestInput>(
        new IRule<TestInput>[] { rule }
    );
    engine.Apply(input);
    Assert.True(input.InputFlag);
  }

  [Fact]
  public void ApplyMany()
  {
    var rule = new TestDefaultPreRule();
    var input = new TestInput();
    var input2 = new TestInput();
    var engine = new RuleEngine<TestInput>(
        new IRule<TestInput>[] { rule }
    );
    engine.Apply(new[] { input, input2 }, new EngineContext());
    Assert.True(input.InputFlag);
    Assert.True(input2.InputFlag);
  }

  [Fact]
  public void ApplyManyHandleEngineHalt()
  {
    var testPreRule = new LambdaRule<TestInput>(
        "test",
        (_, _) => true,
        (_, i) =>
        {
          if (i.InputFlag)
            throw new EngineHaltException();
          i.InputFlag = true;
        });
    var input = new TestInput { InputFlag = true };
    var input2 = new TestInput();
    var engine = new RuleEngine<TestInput>(
        new IRule<TestInput>[] { testPreRule }
    );
    var context = new EngineContext();
    engine.Apply(new[] { input, input2 }, context);
    Assert.True(input.InputFlag);
    Assert.False(input2.InputFlag);
    var ex = context.GetLastException();
    Assert.NotNull(ex);
    Assert.IsType<EngineHaltException>(ex);
    Assert.Equal(testPreRule, ex.Rule);
    Assert.Equal(input, ex.Input);
    Assert.Null(ex.Output);
    Assert.NotNull(ex.Context);
  }

  [Fact]
  public void ApplyManyHandleItemHalt()
  {
    var testPreRule = new LambdaRule<TestInput>(
        "test",
        (_, _) => true,
        (_, i) =>
        {
          if (i.InputFlag)
            throw new ItemHaltException();
          i.InputFlag = true;
        });
    var input = new TestInput { InputFlag = true };
    var input2 = new TestInput();
    var engine = new RuleEngine<TestInput>(
        new IRule<TestInput>[] { testPreRule }
    );
    var context = new EngineContext();
    engine.Apply(new[] { input, input2 }, context);
    Assert.True(input.InputFlag);
    Assert.True(input2.InputFlag);
    var exception = context.GetLastException();
    Assert.NotNull(exception);
    Assert.IsType<ItemHaltException>(exception);
    Assert.Equal(testPreRule, exception.Rule);
    Assert.Equal(input, exception.Input);
    Assert.Null(exception.Output);
    Assert.NotNull(exception.Context);
  }

  [Fact]
  public void ApplyManyHandleExceptionItemHalt()
  {
    var testPreRule = new LambdaRule<TestInput>(
        "test",
        (_, _) => true,
        (_, i) =>
        {
          if (i.InputFlag)
            throw new("Test", null);
          i.InputFlag = true;
        });
    var input = new TestInput { InputFlag = true };
    var input2 = new TestInput();
    var engine = new RuleEngine<TestInput>(
        new IRule<TestInput>[] { testPreRule },
        ExceptionHandlers.HaltItem
    );
    var context = new EngineContext();
    engine.Apply(new[] { input, input2 }, context);
    Assert.True(input.InputFlag);
    Assert.True(input2.InputFlag);
    var exception = context.GetLastException();
    Assert.NotNull(exception);
    Assert.IsType<ItemHaltException>(exception);
    Assert.Equal(testPreRule, exception.Rule);
    Assert.Equal(input, exception.Input);
    Assert.Null(exception.Output);
    Assert.NotNull(exception.Context);
  }

  [Fact]
  public void ApplyManyHandleExceptionEngineHalt()
  {
    var testPreRule = new LambdaRule<TestInput>(
        "test",
        (_, _) => true,
        (_, i) =>
        {
          if (i.InputFlag)
            throw new();
          i.InputFlag = true;
        });
    var input = new TestInput { InputFlag = true };
    var input2 = new TestInput();
    var engine = new RuleEngine<TestInput>(
        new IRule<TestInput>[] { testPreRule },
        ExceptionHandlers.HaltEngine
    );
    var context = new EngineContext();
    engine.Apply(new[] { input, input2 }, context);
    Assert.True(input.InputFlag);
    Assert.False(input2.InputFlag);
    var exception = context.GetLastException();
    Assert.NotNull(exception);
    Assert.IsType<EngineHaltException>(exception);
    Assert.Equal(testPreRule, exception.Rule);
    Assert.Equal(input, exception.Input);
    Assert.Null(exception.Output);
    Assert.NotNull(exception.Context);
  }

  [Fact]
  public void ApplyManyHandleExceptionEngineThrow()
  {
    var testPreRule = new LambdaRule<TestInput>(
        "test",
        (_, _) => true,
        (_, i) =>
        {
          if (i.InputFlag)
            throw new();
          i.InputFlag = true;
        });
    var input = new TestInput { InputFlag = true };
    var input2 = new TestInput();
    var context = new EngineContext();
    var engine = new RuleEngine<TestInput>(
        new IRule<TestInput>[] { testPreRule },
        ExceptionHandlers.Rethrow
    );
    var exception = Assert.Throws<Exception>(() => engine.Apply(new[] { input, input2 }, context));
    Assert.True(input.InputFlag);
    Assert.False(input2.InputFlag);
    Assert.Null(context.GetLastException());
    Assert.IsNotType<EngineHaltException>(exception);
  }

  [Fact]
  public void AppliesOrder()
  {
    var rule = new TestDefaultPreRule();
    var rule2 = new TestPreRule(true, false);
    var input = new TestInput();
    var engine = new RuleEngine<TestInput>(
        new IRule<TestInput>[] { rule, rule2 }
    );
    engine.Apply(input);
    Assert.False(input.InputFlag);
    Assert.Contains(rule, engine.Rules);
    Assert.Contains(rule2, engine.Rules);
  }

  [Fact]
  public void AppliesOrderReverse()
  {
    var rule = new TestDefaultPreRule();
    var rule2 = new TestPreRule(true, false);
    var input = new TestInput { InputFlag = true };
    var output = new TestOutput();
    var engine = new RuleEngine<TestInput, TestOutput>(
        new IRule<TestInput>[] { rule2, rule },
        null,
        null
    );
    engine.Apply(input, output);
    Assert.True(input.InputFlag);
  }

  [Fact]
  public void Constructor()
  {
    var logger = new TestLogger();
    var ruleSet = new Ruleset<TestInput>();
    var engine = new RuleEngine<TestInput>(ruleSet, ExceptionHandlers.Rethrow, logger);
    Assert.Equal(logger, engine.Logger);
    Assert.Empty(engine.Rules);
  }

  [Fact]
  public void ConstructorNullRules()
  {
    var engine = new RuleEngine<TestInput>((IEnumerable<IRule<TestInput>>)null);
    Assert.Empty(engine.Rules);
  }

  [Fact]
  public void ConstructorNullLogger()
  {
    var ruleSet = new Ruleset<TestInput>();
    var engine = new RuleEngine<TestInput>(ruleSet);
    Assert.Equal(NullLogger.Instance, engine.Logger);
  }
  
  [Fact]
  public void ApplyException()
  {
    var testPreRule = new TestExceptionPreRule(false);
    var engine = new RuleEngine<TestInput>(new Rule<TestInput>[] { testPreRule });
    var input = new TestInput();
    var context = new EngineContext();
    Assert.Throws<Exception>(() => engine.Apply(input, context));
    Assert.Null(context.GetLastException());
    Assert.True(input.InputFlag);
  }

  [Fact]
  public void ApplyEngineException()
  {
    var testPreRule = new LambdaRule<TestInput>("test", (_, _) => true, (_, _) => throw new EngineHaltException("Test", null));
    var engine = new RuleEngine<TestInput>(new IRule<TestInput>[] { testPreRule });
    var input = new TestInput();
    var context = new EngineContext();
    engine.Apply(input, context);
    var ex = context.GetLastException();
    Assert.NotNull(ex);
    Assert.IsType<EngineHaltException>(ex);
    Assert.Equal(testPreRule, ex.Rule);
    Assert.Equal(input, ex.Input);
    Assert.Null(ex.Output);
    Assert.NotNull(ex.Context);
    Assert.False(input.InputFlag);
  }

  [Fact]
  public void ApplyItemException()
  {
    var testPreRule = new LambdaRule<TestInput>("test", (_, _) => true, (_, _) => throw new ItemHaltException());
    var engine = new RuleEngine<TestInput>(new IRule<TestInput>[] { testPreRule });
    var input = new TestInput();
    var context = new EngineContext();
    engine.Apply(input, context);
    var ex = context.GetLastException();
    Assert.NotNull(ex);
    Assert.IsType<ItemHaltException>(ex);
    Assert.Equal(testPreRule, ex.Rule);
    Assert.Equal(input, ex.Input);
    Assert.Null(ex.Output);
    Assert.NotNull(ex.Context);
    Assert.False(input.InputFlag);
  }

  [Fact]
  public void ApplyExceptionHandler()
  {
    var testPreRule = new LambdaRule<TestInput>("test", (_, _) => true, (_, _) => throw new());
    var testPreRule2 = new LambdaRule<TestInput>("test2", (_, _) => true, (_, i) => i.InputFlag = true);
    var engine = new RuleEngine<TestInput>(new IRule<TestInput>[] { testPreRule, testPreRule2 }, ExceptionHandlers.HaltEngine);
    var input = new TestInput();
    var context = new EngineContext();
    engine.Apply(input, context);
    var ex = context.GetLastException();
    Assert.NotNull(ex);
    Assert.IsType<EngineHaltException>(ex);
    Assert.Equal(testPreRule, ex.Rule);
    Assert.Equal(input, ex.Input);
    Assert.Null(ex.Output);
    Assert.NotNull(ex.Context);
    Assert.False(input.InputFlag);
  }

  [Fact]
  public void ApplyExceptionHandlerItemException()
  {
    var testPreRule = new LambdaRule<TestInput>("test", (_, _) => true, (_, _) => throw new());
    var testPreRule2 = new LambdaRule<TestInput>("test2", (_, _) => true, (_, i) => i.InputFlag = true);
    var context = new EngineContext();
    var engine = new RuleEngine<TestInput>(new IRule<TestInput>[] { testPreRule, testPreRule2 }, ExceptionHandlers.HaltItem);
    var input = new TestInput();
    engine.Apply(input, context);
    var ex = context.GetLastException();
    Assert.NotNull(ex);
    Assert.IsType<ItemHaltException>(ex);
    Assert.Equal(testPreRule, ex.Rule);
    Assert.Equal(input, ex.Input);
    Assert.Null(ex.Output);
    Assert.NotNull(ex.Context);
    Assert.False(input.InputFlag);
  }

  [Fact]
  public void ApplyExceptionHandlerThrow()
  {
    var testPreRule = new LambdaRule<TestInput>("test", (_, _) => true, (_, _) => throw new());
    var testPreRule2 = new LambdaRule<TestInput>("test2", (_, _) => true, (_, i) => i.InputFlag = true);
    var engine = new RuleEngine<TestInput>(new IRule<TestInput>[] { testPreRule, testPreRule2 }, ExceptionHandlers.Rethrow);
    var input = new TestInput();
    var context = new EngineContext();
    var exception = Assert.Throws<Exception>(() => engine.Apply(input, context));
    Assert.Null(context.GetLastException());
    Assert.IsNotType<EngineException>(exception);
    Assert.False(input.InputFlag);
  }

  [Fact]
  public void ApplyExceptionHandlerThrowException()
  {
    var testPreRule = new LambdaRule<TestInput>("test", (_, _) => true, (_, _) => throw new());
    var testPreRule2 = new LambdaRule<TestInput>("test2", (_, _) => true, (_, i) => i.InputFlag = true);
    var engine = new RuleEngine<TestInput>(new IRule<TestInput>[] { testPreRule, testPreRule2 },
        new LambdaExceptionHandler((_, _, _, _, _) => throw new InvalidOperationException()));
    var input = new TestInput();
    var context = new EngineContext();
    Assert.Throws<InvalidOperationException>(() => engine.Apply(input, context));
    Assert.Null(context.GetLastException());
    Assert.False(input.InputFlag);
  }

  [Fact]
  public void ApplyExceptionHandlerIgnore()
  {
    var testPreRule = new LambdaRule<TestInput>("test", (_, _) => true, (_, _) => throw new());
    var testPreRule2 = new LambdaRule<TestInput>("test2", (_, _) => true, (_, i) => i.InputFlag = true);
    var engine = new RuleEngine<TestInput>(new IRule<TestInput>[] { testPreRule, testPreRule2 }, ExceptionHandlers.Ignore);
    var input = new TestInput();
    var context = new EngineContext();
    engine.Apply(input, context);
    Assert.Null(context.GetLastException());
    Assert.True(input.InputFlag);
  }

  [Fact]
  public void DoesApplyException()
  {
    var testPreRule = new TestExceptionPreRule(true);
    var engine = new RuleEngine<TestInput>(new Rule<TestInput>[] { testPreRule }, ExceptionHandlers.Rethrow);
    var input = new TestInput();
    var context = new EngineContext();
    var exception = Assert.Throws<Exception>(() => engine.Apply(input, context));
    Assert.Null(context.GetLastException());
    Assert.IsNotType<EngineException>(exception);
    Assert.False(input.InputFlag);
  }

  [Fact]
  public void NotApplies()
  {
    var rule = new TestPreRule(false);
    var input = new TestInput();
    var engine = new RuleEngine<TestInput>(
        new IRule<TestInput>[] { rule }
    );
    engine.Apply(input);
    Assert.False(input.InputFlag);
  }

  [Fact]
  public void Properties()
  {
    var logger = new TestLogger();
    var ruleSet = new Ruleset<TestInput>();
    var engine = new RuleEngine<TestInput>(ruleSet, ExceptionHandlers.Rethrow, logger);
    Assert.False(engine.IsAsync);
    Assert.Equal(typeof(TestInput), engine.InputType);
    Assert.Equal(typeof(TestInput), engine.OutputType);
    Assert.Equal(ExceptionHandlers.Rethrow, engine.ExceptionHandler);
  }

}
