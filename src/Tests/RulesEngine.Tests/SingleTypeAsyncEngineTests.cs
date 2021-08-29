using RulesEngine.Tests.TestRules;
using RulesEngine.Tests.TestRules.Async;

namespace RulesEngine.Tests;

public class SingleTypeAsyncEngineTests
{
  [Fact]
  public async Task AppliesOrder()
  {
    var rule = new TestDefaultAsyncPreRule();
    var rule2 = new TestAsyncPreRule(true, false);
    var input = new TestInput();
    var engine = new AsyncRulesEngine<TestInput>(
        new IAsyncRule<TestInput>[] { rule, rule2 },
        false,
        null
    );
    await engine.ApplyAsync(input);
    Assert.False(input.InputFlag);
  }

  [Fact]
  public async Task AppliesOrderReverse()
  {
    var rule = new TestDefaultAsyncPreRule();
    var rule2 = new TestAsyncPreRule(true, false);
    var input = new TestInput { InputFlag = true };
    var engine = new AsyncRulesEngine<TestInput>(
        new IAsyncRule<TestInput>[] { rule2, rule }, false, null
    );
    await engine.ApplyAsync(input);
    Assert.True(input.InputFlag);
  }

  [Fact]
  public void Constructor()
  {
    var logger = new TestLogger();
    var ruleSet = new AsyncRuleset<TestInput>();
    var engine = new AsyncRulesEngine<TestInput>(ruleSet, false, ExceptionHandlers.HaltEngine, logger);
    Assert.Equal(logger, engine.Logger);
    Assert.False(engine.IsParallel);
    Assert.Equal(ExceptionHandlers.HaltEngine, engine.ExceptionHandler);
  }

  [Fact]
  public void Properties()
  {
    var logger = new TestLogger();
    var ruleSet = new AsyncRuleset<TestInput>();
    var engine = new AsyncRulesEngine<TestInput>(ruleSet, false, null, logger);
    Assert.True(engine.IsAsync);
    Assert.False(engine.IsParallel);
    Assert.Equal(typeof(TestInput), engine.InputType);
    Assert.Equal(typeof(TestInput), engine.OutputType);
    Assert.Equal(ExceptionHandlers.Throw, engine.ExceptionHandler);
  }

  [Fact]
  public void PropertiesParallel()
  {
    var logger = new TestLogger();
    var ruleSet = new AsyncRuleset<TestInput>();
    var engine = new AsyncRulesEngine<TestInput>(ruleSet, true, null, logger);
    Assert.True(engine.IsAsync);
    Assert.True(engine.IsParallel);
    Assert.Equal(typeof(TestInput), engine.InputType);
  }

  [Fact]
  public void ConstructorNullLogger()
  {
    var ruleSet = new AsyncRuleset<TestInput>();
    var engine = new AsyncRulesEngine<TestInput>(ruleSet);
    Assert.NotNull(engine.Logger);
  }

  [Fact]
  public void ConstructorParallel()
  {
    var ruleSet = new AsyncRuleset<TestInput>();
    var engine = new AsyncRulesEngine<TestInput>(ruleSet, true);
    Assert.True(engine.IsParallel);
  }

  [Fact]
  public void ConstructorWithEmptySyncRuleset()
  {
    var logger = new TestLogger();
    var ruleSet = new Ruleset<TestInput>();
    var engine = new AsyncRulesEngine<TestInput>(ruleSet, false, null, logger);
    Assert.Equal(logger, engine.Logger);
  }

  [Fact]
  public void ConstructorWithSyncRuleset()
  {
    var logger = new TestLogger();
    var ruleSet = new Ruleset<TestInput>();
    ruleSet.AddRule(new TestPreRule(true));
    var engine = new AsyncRulesEngine<TestInput>(ruleSet, false, null, logger);
    Assert.NotEmpty(engine.Rules);
  }


  [Fact]
  public async Task Applies()
  {
    var rule = new TestDefaultAsyncPreRule();
    var input = new TestInput();
    var engine = new AsyncRulesEngine<TestInput>(
        new IAsyncRule<TestInput>[] { rule },
        false,
        null
    );
    await engine.ApplyAsync(input);
    Assert.True(input.InputFlag);
  }

  [Fact]
  public async Task AppliesMany()
  {
    var rule = new TestDefaultAsyncPreRule();
    var input = new TestInput();
    var input2 = new TestInput();
    var engine = new AsyncRulesEngine<TestInput>(
        new IAsyncRule<TestInput>[] { rule },
        false,
        null
    );
    await engine.ApplyAsync(new TestInput[] { input, input2 });
    Assert.True(input.InputFlag);
    Assert.True(input2.InputFlag);
  }

  [Fact]
  public async Task AppliesManyEmpty()
  {
    var rule = new TestDefaultAsyncPreRule();
    var engine = new AsyncRulesEngine<TestInput>(
        new IAsyncRule<TestInput>[] { rule },
        false,
        null
    );
    await engine.ApplyAsync(Array.Empty<TestInput>());
    //Shouldn't throw
  }

  [Fact]
  public async Task NotApplies()
  {
    var rule = new TestAsyncPreRule(false);
    var input = new TestInput();
    var engine = new AsyncRulesEngine<TestInput>(
        new IAsyncRule<TestInput>[] { rule },
        false,
        null
    );
    await engine.ApplyAsync(input);
    Assert.False(input.InputFlag);
  }

  [Fact]
  public async Task ApplyAsyncException()
  {
    var testPreRule = new TestExceptionAsyncPreRule(false);
    var engine =
        new AsyncRulesEngine<TestInput, TestOutput>(
            new AsyncRule<TestInput>[] { testPreRule }, null, null);
    var input = new TestInput();
    var output = new TestOutput();
    var exception = await Assert.ThrowsAsync<Exception>(() => engine.ApplyAsync(input, output));
    Assert.IsNotType<EngineException>(exception);
    Assert.Null(engine.LastException);
    Assert.True(input.InputFlag);
  }

  [Fact]
  public async Task DoesApplyAsyncException()
  {
    var testPreRule = new TestExceptionAsyncPreRule(false);
    var engine =
        new AsyncRulesEngine<TestInput>(
            new AsyncRule<TestInput>[] { testPreRule }, false, null);
    var input = new TestInput();
    var output = new TestOutput();
    var exception = await Assert.ThrowsAsync<Exception>(async () => await engine.ApplyAsync(input));
    Assert.IsNotType<EngineException>(exception);
    Assert.Null(engine.LastException);
    Assert.True(input.InputFlag);
  }
}
