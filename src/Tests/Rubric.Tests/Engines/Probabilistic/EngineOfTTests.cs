using Microsoft.Extensions.Logging.Abstractions;
using Rubric.Engines.Probabilistic;
using Rubric.Engines.Probabilistic.Implementation;
using Rubric.Rules.Probabilistic;
using Rubric.Rulesets.Probabilistic;
using Rubric.Tests.TestRules.Probabilistic;

namespace Rubric.Tests.Engines.Probabilistic;

public class EngineOfTTests
{
  [Fact]
  public void EmptyRuleset()
  {
    var engine = new RuleEngine<TestInput>((IRuleset<TestInput>)null);
    Assert.Empty(engine.Rules);
  }

  [Fact]
  public void NullList()
  {
    var engine = new RuleEngine<TestInput>((IRuleset<TestInput>)null);
    Assert.Throws<ArgumentNullException>(() => engine.Apply((IEnumerable<TestInput>)null));
  }

  [Fact]
  public void NullItem()
  {
    var engine = new RuleEngine<TestInput>((IRuleset<TestInput>)null);
    Assert.Throws<ArgumentNullException>(() => engine.Apply((TestInput)null));
  }

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
        (_, _) => 1D,
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
        (_, _) => 1D,
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
        (_, _) => 1D,
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
        (_, _) => 1D,
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
        (_, _) => 1D,
        (_, i) =>
        {
          if (i.InputFlag)
            throw new();
          i.InputFlag = true;
        });
    var input = new TestInput { InputFlag = true };
    var input2 = new TestInput();
    var context = new EngineContext();
    IRuleEngine<TestInput> engine = new RuleEngine<TestInput>(
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
    var rule2 = new TestPreRule(1D, false);
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
    var rule2 = new TestPreRule(1D, false);
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
    var testPreRule = new LambdaRule<TestInput>("test", (_, _) => 1D, (_, _) => throw new EngineHaltException("Test", null));
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
    var testPreRule = new LambdaRule<TestInput>("test", (_, _) => 1D, (_, _) => throw new ItemHaltException());
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
    var testPreRule = new LambdaRule<TestInput>("test", (_, _) => 1D, (_, _) => throw new());
    var testPreRule2 = new LambdaRule<TestInput>("test2", (_, _) => 1D, (_, i) => i.InputFlag = true);
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
    var testPreRule = new LambdaRule<TestInput>("test", (_, _) => 1D, (_, _) => throw new());
    var testPreRule2 = new LambdaRule<TestInput>("test2", (_, _) => 1D, (_, i) => i.InputFlag = true);
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
    var testPreRule = new LambdaRule<TestInput>("test", (_, _) => 1D, (_, _) => throw new());
    var testPreRule2 = new LambdaRule<TestInput>("test2", (_, _) => 1D, (_, i) => i.InputFlag = true);
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
    var testPreRule = new LambdaRule<TestInput>("test", (_, _) => 1D, (_, _) => throw new());
    var testPreRule2 = new LambdaRule<TestInput>("test2", (_, _) => 1D, (_, i) => i.InputFlag = true);
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
    var testPreRule = new LambdaRule<TestInput>("test", (_, _) => 1D, (_, _) => throw new());
    var testPreRule2 = new LambdaRule<TestInput>("test2", (_, _) => 1D, (_, i) => i.InputFlag = true);
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
    var rule = new TestPreRule(0D);
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

  [Fact]
  public void ExceptionHandlingException()
  {
    var logger = new TestLogger();
    var ruleSet = new Ruleset<TestInput>();
    var engine = new RuleEngine<TestInput>(ruleSet, ExceptionHandlers.Rethrow, logger);
    Assert.Throws<ArgumentNullException>(() => Rubric.Engines.Implementation.BaseRuleEngine.HandleException(new(), null, new EngineContext(), null, null, null));
    Assert.Throws<ArgumentNullException>(() => Rubric.Engines.Implementation.BaseRuleEngine.HandleException(new(), engine, null, null, null, null));
  }

  [Fact]
  public void PerItemCaching()
  {
    var engine =
      ProbabilisticEngineBuilder.ForInput<TestInput>()
                                .WithRule("cacherule1")
                                .WithPredicate((_, i) => ++i.Counter > 0 ? 1 : 0)
                                .WithAction((_, i) => i.Items.Add(""))
                                .WithCaching(new(CacheBehavior.PerInput, "testkey"))
                                .EndRule()
                                .WithRule("cacherule2")
                                .WithPredicate((_, i) => ++i.Counter > 0 ? 1 : 0)
                                .WithAction((_, i) => i.Items.Add(""))
                                .WithCaching(new(CacheBehavior.PerInput, "testkey"))
                                .EndRule()
                                .Build();
    //Only one predicate should execute, but both actions should execute.  Both items should be processed identically
    var items = new[] { new TestInput(), new TestInput() };
    var context = new EngineContext();
    engine.Apply(items, context);
    foreach (var item in items)
    {
      Assert.Equal(1, item.Counter);
      Assert.Equal(2, item.Items.Count);
    }
    Assert.Empty(context.GetInputPredicateCache());
    Assert.Empty(context.GetExecutionPredicateCache());
  }

  [Fact]
  public void PerExecutionCaching()
  {
    var engine =
      ProbabilisticEngineBuilder.ForInput<TestInput>()
                   .WithRule("cacherule1")
                   .WithPredicate((_, i) => ++i.Counter > 0 ? 1 : 0)
                   .WithAction((_, i) => i.Items.Add(""))
                   .WithCaching(new(CacheBehavior.PerExecution, "testkey"))
                   .EndRule()
                   .WithRule("cacherule2")
                   .WithPredicate((_, i) => ++i.Counter > 0 ? 1 : 0)
                   .WithAction((_, i) => i.Items.Add(""))
                   .WithCaching(new(CacheBehavior.PerExecution, "testkey"))
                   .EndRule()
                   .Build();
    //Only one predicate should execute, but both actions should execute.  Both items should be processed identically
    var items = new[] { new TestInput(), new TestInput() };
    var context = new EngineContext();
    engine.Apply(items, context);
    Assert.Equal(1, items[0].Counter);
    Assert.Equal(2, items[0].Items.Count);
    Assert.Equal(0, items[1].Counter);
    Assert.Equal(2, items[1].Items.Count);

    Assert.Empty(context.GetInputPredicateCache());
    Assert.Empty(context.GetExecutionPredicateCache());
  }

  [Fact]
  public void CachesClearedOnItemHalt()
  {
    var engine =
      ProbabilisticEngineBuilder.ForInput<TestInput>()
                                .WithRule("cacherule1")
                                .WithPredicate((_, i) => ++i.Counter > 0 ? 1 : 0)
                                .WithAction((_, i) => i.Items.Add(""))
                                .WithCaching(new(CacheBehavior.PerExecution, "testkey"))
                                .EndRule()
                                .WithRule("cacherule2")
                                .WithPredicate((_, i) => ++i.Counter > 0 ? 1 : 0)
                                .WithAction((_, _) => throw new ItemHaltException())
                                .WithCaching(new(CacheBehavior.PerExecution, "testkey"))
                                .EndRule()
                                .Build();
    //Only one predicate should execute, but both actions should execute.  Both items should be processed identically
    var items = new[] { new TestInput(), new TestInput() };
    var context = new EngineContext();
    engine.Apply(items, context);
    Assert.Empty(context.GetInputPredicateCache());
    Assert.Empty(context.GetExecutionPredicateCache());
  }

  [Fact]
  public void CachesClearedOnEngineHalt()
  {
    var engine =
      ProbabilisticEngineBuilder.ForInput<TestInput>()
                                .WithRule("cacherule1")
                                .WithPredicate((_, i) => ++i.Counter > 0 ? 1 : 0)
                                .WithAction((_, i) => i.Items.Add(""))
                                .WithCaching(new(CacheBehavior.PerExecution, "testkey"))
                                .EndRule()
                                .WithRule("cacherule2")
                                .WithPredicate((_, i) => ++i.Counter > 0 ? 1 : 0)
                                .WithAction((_, _) => throw new EngineHaltException())
                                .WithCaching(new(CacheBehavior.PerExecution, "testkey"))
                                .EndRule()
                                .Build();
    //Only one predicate should execute, but both actions should execute.  Both items should be processed identically
    var items = new[] { new TestInput(), new TestInput() };
    var context = new EngineContext();
    engine.Apply(items, context);
    Assert.Empty(context.GetInputPredicateCache());
    Assert.Empty(context.GetExecutionPredicateCache());
  }
}
