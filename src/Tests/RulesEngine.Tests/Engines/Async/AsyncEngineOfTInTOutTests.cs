using RulesEngine.Tests.TestRules;
using RulesEngine.Tests.TestRules.Async;

namespace RulesEngine.Tests.Engines.Async;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
public class AsyncEngineOfTInTOutTests
{

  [Fact]
  public async Task Applies()
  {
    var rule = new TestDefaultAsyncRule();
    var input = new TestInput();
    var output = new TestOutput();
    var engine = new AsyncRulesEngine<TestInput, TestOutput>(
        null,
        new IAsyncRule<TestInput, TestOutput>[] { rule },
        null
    );
    await engine.ApplyAsync(input, output);
    Assert.True(input.InputFlag);
    Assert.True(output.TestFlag);
  }

  [Fact]
  public async Task AppliesOrder()
  {
    var rule = new TestDefaultAsyncPreRule();
    var rule2 = new TestAsyncPreRule(true, false);
    var input = new TestInput();
    var output = new TestOutput();
    var engine = new AsyncRulesEngine<TestInput, TestOutput>(
        new IAsyncRule<TestInput>[] { rule, rule2 },
        null,
        null
    );
    await engine.ApplyAsync(input, output);
    Assert.False(input.InputFlag);
  }

  [Fact]
  public async Task AppliesOrderReverse()
  {
    var rule = new TestDefaultAsyncPreRule();
    var rule2 = new TestAsyncPreRule(true, false);
    var input = new TestInput { InputFlag = true };
    var output = new TestOutput();
    var engine = new AsyncRulesEngine<TestInput, TestOutput>(
        new IAsyncRule<TestInput>[] { rule2, rule },
        null,
        null
    );
    await engine.ApplyAsync(input, output);
    Assert.True(input.InputFlag);
  }

  [Fact]
  public void Constructor()
  {
    var logger = new TestLogger();
    var ruleSet = new AsyncRuleset<TestInput, TestOutput>();
    var engine = new AsyncRulesEngine<TestInput, TestOutput>(ruleSet, false, null, logger);
    Assert.Equal(logger, engine.Logger);
    Assert.False(engine.IsParallel);
  }

  [Fact]
  public void ConstructorNullLogger()
  {
    var ruleSet = new AsyncRuleset<TestInput, TestOutput>();
    var engine = new AsyncRulesEngine<TestInput, TestOutput>(ruleSet);
    Assert.NotNull(engine.Logger);
  }

  [Fact]
  public void ConstructorParallel()
  {
    var ruleSet = new AsyncRuleset<TestInput, TestOutput>();
    var engine = new AsyncRulesEngine<TestInput, TestOutput>(ruleSet, true);
    Assert.True(engine.IsParallel);
  }

  [Fact]
  public void ConstructorWithEmptySyncRuleset()
  {
    var logger = new TestLogger();
    var ruleSet = new Ruleset<TestInput, TestOutput>();
    var engine = new AsyncRulesEngine<TestInput, TestOutput>(ruleSet, false, null, logger);
    Assert.Equal(logger, engine.Logger);
  }

  [Fact]
  public void ConstructorWithSyncRuleset()
  {
    var logger = new TestLogger();
    var ruleSet = new Ruleset<TestInput, TestOutput>();
    ruleSet.AddPreRule(new TestPreRule(true));
    ruleSet.AddPostRule(new TestPostRule(true));
    ruleSet.AddRule(new TestRule(true));
    var engine = new AsyncRulesEngine<TestInput, TestOutput>(ruleSet, false, null, logger);
    Assert.NotEmpty(engine.PreRules);
    Assert.NotEmpty(engine.Rules);
    Assert.NotEmpty(engine.PostRules);
  }

  [Fact]
  public async Task FullRun()
  {
    var engine = EngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                              .WithPreRule("test")
                              .WithPredicate((c, i) => Task.FromResult(true))
                              .WithAction((c, i) =>
                              {
                                i.Items.Add("pre");
                                return Task.CompletedTask;
                              })
                              .EndRule()
                              .WithRule("test")
                              .WithPredicate((c, i, o) => Task.FromResult(true))
                              .WithAction((c, i, o) =>
                              {
                                i.Items.Add("rule");
                                o.Outputs.Add("rule");
                                return Task.CompletedTask;
                              })
                              .EndRule()
                              .WithPostRule("test")
                              .WithPredicate((c, o) => Task.FromResult(true))
                              .WithAction((c, o) =>
                              {
                                o.Outputs.Add("postrule");
                                return Task.CompletedTask;
                              })
                              .EndRule()
                              .Build();
    var input1 = new TestInput();
    var input2 = new TestInput();
    var output = new TestOutput();
    await engine.ApplyAsync(new[] { input1, input2 }, output);
    Assert.Equal(2, input1.Items.Count);
    Assert.Contains("pre", input1.Items);
    Assert.Contains("rule", input1.Items);
    Assert.Equal(2, input2.Items.Count);
    Assert.Contains("pre", input2.Items);
    Assert.Contains("rule", input2.Items);
    Assert.Equal(3, output.Outputs.Count);
    Assert.Equal(2, output.Outputs.Count(o => o == "rule"));
    Assert.Single(output.Outputs.Where(o => o == "postrule"));
  }

  [Fact]
  public async Task NotApplies()
  {
    var rule = new TestAsyncRule(false);
    var input = new TestInput();
    var output = new TestOutput();
    var engine = new AsyncRulesEngine<TestInput, TestOutput>(
        null,
        new IAsyncRule<TestInput, TestOutput>[] { rule },
        null
    );
    await engine.ApplyAsync(input, output);
    Assert.False(input.InputFlag);
    Assert.False(output.TestFlag);
  }

  [Fact]
  public async Task PostApplies()
  {
    var rule = new TestDefaultAsyncPostRule();
    var input = new TestInput();
    var output = new TestOutput();
    var engine = new AsyncRulesEngine<TestInput, TestOutput>(
        null,
        null,
        new IAsyncRule<TestOutput>[] { rule }
    );
    await engine.ApplyAsync(input, output);
    Assert.True(output.TestFlag);
  }

  [Fact]
  public async Task PostNotApplies()
  {
    var rule = new TestAsyncPostRule(false);
    var input = new TestInput();
    var output = new TestOutput();
    var engine = new AsyncRulesEngine<TestInput, TestOutput>(
        null,
        null,
        new IAsyncRule<TestOutput>[] { rule }
    );
    await engine.ApplyAsync(input, output);
    Assert.False(output.TestFlag);
  }

  [Fact]
  public async Task PreApplies()
  {
    var rule = new TestDefaultAsyncPreRule();
    var input = new TestInput();
    var output = new TestOutput();
    var engine = new AsyncRulesEngine<TestInput, TestOutput>(
        new IAsyncRule<TestInput>[] { rule },
        null,
        null
    );
    await engine.ApplyAsync(input, output);
    Assert.True(input.InputFlag);
  }

  [Fact]
  public async Task PreNotApplies()
  {
    var rule = new TestAsyncPreRule(false);
    var input = new TestInput();
    var output = new TestOutput();
    var engine = new AsyncRulesEngine<TestInput, TestOutput>(
        new IAsyncRule<TestInput>[] { rule },
        null,
        null
    );
    await engine.ApplyAsync(input, output);
    Assert.False(input.InputFlag);
  }

  [Fact]
  public async Task ApplyAsyncException()
  {
    var engine = new AsyncRulesEngine<TestInput, TestOutput>(
        null, new AsyncRule<TestInput, TestOutput>[] { new TestExceptionAsyncRule(false) }, null);
    var input = new TestInput();
    var output = new TestOutput();
    var exception = await Assert.ThrowsAsync<Exception>(() => engine.ApplyAsync(input, output));
    Assert.IsNotType<EngineException>(exception);
    Assert.Null(engine.LastException);
    Assert.True(input.InputFlag);
    Assert.True(output.TestFlag);
  }

  [Fact]
  public async Task DoesApplyAsyncException()
  {
    var engine = new AsyncRulesEngine<TestInput, TestOutput>(
        null, new AsyncRule<TestInput, TestOutput>[] { new TestExceptionAsyncRule(true) }, null);
    var input = new TestInput();
    var output = new TestOutput();
    var exception = await Assert.ThrowsAsync<Exception>(() => engine.ApplyAsync(input, output));
    Assert.IsNotType<EngineException>(exception);
    Assert.Null(engine.LastException);
    Assert.False(input.InputFlag);
    Assert.False(output.TestFlag);
  }

  [Fact]
  public async Task PostApplyAsyncException()
  {
    var testPostRule = new TestExceptionAsyncPostRule(false);
    var engine =
        new AsyncRulesEngine<TestInput, TestOutput>(
            null, null, new AsyncRule<TestOutput>[] { testPostRule });
    var input = new TestInput();
    var output = new TestOutput();
    var exception = await Assert.ThrowsAsync<Exception>(() => engine.ApplyAsync(input, output));
    Assert.True(output.TestFlag);
    Assert.IsNotType<EngineException>(exception);
    Assert.Null(engine.LastException);
  }

  [Fact]
  public async Task PostDoesApplyAsyncException()
  {
    var testPostRule = new TestExceptionAsyncPostRule(false);
    var engine =
        new AsyncRulesEngine<TestInput, TestOutput>(
            null, null, new AsyncRule<TestOutput>[] { testPostRule });
    var input = new TestInput();
    var output = new TestOutput();
    var exception = await Assert.ThrowsAsync<Exception>(() => engine.ApplyAsync(input, output));
    Assert.IsNotType<EngineException>(exception);
    Assert.Null(engine.LastException);
    Assert.True(output.TestFlag);
  }

  [Fact]
  public async Task PreApplyAsyncException()
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
  public async Task PreDoesApplyAsyncException()
  {
    var testPreRule = new TestExceptionAsyncPreRule(false);
    var engine =
        new AsyncRulesEngine<TestInput, TestOutput>(
            new AsyncRule<TestInput>[] { testPreRule }, null, null);
    var input = new TestInput();
    var output = new TestOutput();
    var exception = await Assert.ThrowsAsync<Exception>(async () => await engine.ApplyAsync(input, output));
    Assert.IsNotType<EngineException>(exception);
    Assert.Null(engine.LastException);
    Assert.True(input.InputFlag);
  }

  [Fact]
  public async Task ExceptionHandlingIgnore()
  {
    var testInput = new TestInput()
    {
      Items = new() { "PreException", "Exception" }
    };
    var testInput2 = new TestInput();
    var testOutput = new TestOutput()
    {
      Outputs = new() { "PostException" }
    };
    var engine = GetExceptionEngine(ExceptionHandlers.Ignore);
    await engine.ApplyAsync(new[] { testInput, testInput2 }, testOutput);
    Assert.Null(engine.LastException);
    Assert.Equal(4, testInput.Items.Count);
    Assert.Equal(4, testInput2.Items.Count);
    Assert.Equal(2, testOutput.Outputs.Count);

  }

  [Fact]
  public async Task ExceptionHandlingPreThrow()
  {
    var testInput = new TestInput()
    {
      Items = new() { "PreException", "Exception" }
    };
    var testInput2 = new TestInput();
    var testOutput = new TestOutput()
    {
    };
    var engine = GetExceptionEngine(ExceptionHandlers.Throw);
    await Assert.ThrowsAsync<Exception>(() => engine.ApplyAsync(new[] { testInput, testInput2 }, testOutput));
    Assert.Null(engine.LastException);
    Assert.Equal(3, testInput.Items.Count);
    Assert.Empty(testInput2.Items);
    Assert.Empty(testOutput.Outputs);

  }

  [Fact]
  public async Task ExceptionHandlingPreItem()
  {
    var testInput = new TestInput()
    {
      Items = new() { "PreException", "Exception" }
    };
    var testInput2 = new TestInput();
    var testOutput = new TestOutput();
    var engine = GetExceptionEngine(ExceptionHandlers.HaltItem);
    await engine.ApplyAsync(new[] { testInput, testInput2 }, testOutput);
    Assert.NotNull(engine.LastException);
    Assert.IsType<ItemHaltException>(engine.LastException);
    Assert.Equal(testInput, engine.LastException.Input);
    Assert.Equal(engine.PreRules.First(), engine.LastException.Rule);
    Assert.NotNull(engine.LastException.Context);
    Assert.Equal(3, testInput.Items.Count);
    Assert.Equal(4, testInput2.Items.Count);
    Assert.Equal(2, testOutput.Outputs.Count);

  }

  [Fact]
  public async Task ExceptionHandlingPreEngine()
  {
    var testInput = new TestInput()
    {
      Items = new() { "PreException", "Exception" }
    };
    var testInput2 = new TestInput();
    var testOutput = new TestOutput();
    var engine = GetExceptionEngine(ExceptionHandlers.HaltEngine);
    await engine.ApplyAsync(new[] { testInput, testInput2 }, testOutput);
    Assert.NotNull(engine.LastException);
    Assert.IsType<EngineHaltException>(engine.LastException);
    Assert.Equal(testInput, engine.LastException.Input);
    Assert.Equal(engine.PreRules.First(), engine.LastException.Rule);
    Assert.NotNull(engine.LastException.Context);
    Assert.Equal(3, testInput.Items.Count);
    Assert.Empty(testInput2.Items);
    Assert.Empty(testOutput.Outputs);
  }

  [Fact]
  public async Task ExceptionHandlingPreHandlerException()
  {
    var testInput = new TestInput()
    {
      Items = new() { "PreException", "Exception" }
    };
    var testInput2 = new TestInput();
    var testOutput = new TestOutput();
    var engine = GetExceptionEngine(new LambdaExceptionHandler(
      (e, c, i, o, r) => throw new InvalidOperationException()));
    await Assert.ThrowsAsync<InvalidOperationException>(() => engine.ApplyAsync(new[] { testInput, testInput2 }, testOutput));
    Assert.Null(engine.LastException);
    Assert.Equal(3, testInput.Items.Count);
    Assert.Empty(testInput2.Items);
    Assert.Empty(testOutput.Outputs);
  }

  [Fact]
  public async Task ExceptionHandlingHandlerException()
  {
    var testInput = new TestInput()
    {
      Items = new() { "Exception" }
    };
    var testInput2 = new TestInput();
    var testOutput = new TestOutput();
    var engine = GetExceptionEngine(new LambdaExceptionHandler(
      (e, c, i, o, r) => throw new InvalidOperationException()));
    await Assert.ThrowsAsync<InvalidOperationException>(() => engine.ApplyAsync(new[] { testInput, testInput2 }, testOutput));
    Assert.Null(engine.LastException);
    Assert.Equal(4, testInput.Items.Count);
    Assert.Empty(testInput2.Items);
    Assert.Empty(testOutput.Outputs);
  }

  [Fact]
  public async Task ExceptionHandlingPreManualItem()
  {
    var testInput = new TestInput()
    {
      Items = new() { "PreException" }
    };
    var testInput2 = new TestInput();
    var testOutput = new TestOutput();
    var engine = GetEngineExceptionEngine<ItemHaltException>();
    await engine.ApplyAsync(new[] { testInput, testInput2 }, testOutput);
    Assert.NotNull(engine.LastException);
    Assert.IsType<ItemHaltException>(engine.LastException);
    Assert.Equal(testInput, engine.LastException.Input);
    Assert.Equal(engine.PreRules.First(), engine.LastException.Rule);
    Assert.NotNull(engine.LastException.Context);
    Assert.Equal(2, testInput.Items.Count);
    Assert.Equal(4, testInput2.Items.Count);
    Assert.Equal(2, testOutput.Outputs.Count);
  }

  [Fact]
  public async Task ExceptionHandlingManualEngine()
  {
    var testInput = new TestInput()
    {
      Items = new() { "Exception" }
    };
    var testInput2 = new TestInput();
    var testOutput = new TestOutput();
    var engine = GetEngineExceptionEngine<EngineHaltException>();
    await engine.ApplyAsync(new[] { testInput, testInput2 }, testOutput);
    Assert.NotNull(engine.LastException);
    Assert.IsType<EngineHaltException>(engine.LastException);
    Assert.Equal(testInput, engine.LastException.Input);
    Assert.Equal(engine.Rules.First(), engine.LastException.Rule);
    Assert.NotNull(engine.LastException.Context);
    Assert.Equal(4, testInput.Items.Count);
    Assert.Empty(testInput2.Items);
    Assert.Empty(testOutput.Outputs);
  }

  [Fact]
  public async Task ExceptionHandlingManualItem()
  {
    var testInput = new TestInput()
    {
      Items = new() { "Exception" }
    };
    var testInput2 = new TestInput();
    var testOutput = new TestOutput();
    var engine = GetEngineExceptionEngine<ItemHaltException>();
    await engine.ApplyAsync(new[] { testInput, testInput2 }, testOutput);
    Assert.NotNull(engine.LastException);
    Assert.IsType<ItemHaltException>(engine.LastException);
    Assert.Equal(testInput, engine.LastException.Input);
    Assert.Equal(engine.Rules.First(), engine.LastException.Rule);
    Assert.NotNull(engine.LastException.Context);
    Assert.Equal(4, testInput.Items.Count);
    Assert.Equal(4, testInput2.Items.Count);
    Assert.Equal(2, testOutput.Outputs.Count);
  }

  [Fact]
  public async Task ExceptionHandlingPreManualEngine()
  {
    var testInput = new TestInput()
    {
      Items = new() { "PreException" }
    };
    var testInput2 = new TestInput();
    var testOutput = new TestOutput();
    var engine = GetEngineExceptionEngine<EngineHaltException>();
    await engine.ApplyAsync(new[] { testInput, testInput2 }, testOutput);
    Assert.NotNull(engine.LastException);
    Assert.IsType<EngineHaltException>(engine.LastException);
    Assert.Equal(testInput, engine.LastException.Input);
    Assert.Equal(engine.PreRules.First(), engine.LastException.Rule);
    Assert.NotNull(engine.LastException.Context);
    Assert.NotNull(engine.LastException);
    Assert.IsType<EngineHaltException>(engine.LastException);
    Assert.Equal(testInput, engine.LastException.Input);
    Assert.Equal(engine.PreRules.First(), engine.LastException.Rule);
    Assert.NotNull(engine.LastException.Context);
    Assert.Equal(2, testInput.Items.Count);
    Assert.Empty(testInput2.Items);
    Assert.Empty(testOutput.Outputs);
  }

  [Fact]
  public async Task ExceptionHandlingPostManualEngine()
  {
    var testInput = new TestInput()
    {
    };
    var testInput2 = new TestInput();
    var testOutput = new TestOutput()
    {
      Outputs = new() { "PostException" }
    };
    var engine = GetEngineExceptionEngine<EngineHaltException>();
    await engine.ApplyAsync(new[] { testInput, testInput2 }, testOutput);
    Assert.NotNull(engine.LastException);
    Assert.IsType<EngineHaltException>(engine.LastException);
    Assert.Equal(engine.PostRules.First(), engine.LastException.Rule);
    Assert.NotNull(engine.LastException.Context);
    Assert.NotNull(engine.LastException);
    Assert.IsType<EngineHaltException>(engine.LastException);
    Assert.Equal(4, testInput.Items.Count);
    Assert.Equal(4, testInput2.Items.Count);
    Assert.Equal(2, testOutput.Outputs.Count);
  }

  [Fact]
  public async Task ExceptionHandlingPostManualItem()
  {
    var testInput = new TestInput()
    {
    };
    var testInput2 = new TestInput();
    var testOutput = new TestOutput()
    {
      Outputs = new() { "PostException" }
    };
    var engine = GetEngineExceptionEngine<ItemHaltException>();
    await engine.ApplyAsync(new[] { testInput, testInput2 }, testOutput);
    Assert.NotNull(engine.LastException);
    Assert.IsType<ItemHaltException>(engine.LastException);
    Assert.Equal(engine.PostRules.First(), engine.LastException.Rule);
    Assert.NotNull(engine.LastException.Context);
    Assert.NotNull(engine.LastException);
    Assert.IsType<ItemHaltException>(engine.LastException);
    Assert.Equal(4, testInput.Items.Count);
    Assert.Equal(4, testInput2.Items.Count);
    Assert.Equal(2, testOutput.Outputs.Count);
  }

  [Fact]
  public async Task ExceptionHandlingPostManualEngineSingleItem()
  {
    var testInput = new TestInput();
    var testOutput = new TestOutput()
    {
      Outputs = new() { "PostException" }
    };
    var engine = GetEngineExceptionEngine<EngineHaltException>();
    await engine.ApplyAsync(testInput, testOutput);
    Assert.NotNull(engine.LastException);
    Assert.IsType<EngineHaltException>(engine.LastException);
    Assert.Equal(engine.PostRules.First(), engine.LastException.Rule);
    Assert.NotNull(engine.LastException.Context);
    Assert.NotNull(engine.LastException);
    Assert.IsType<EngineHaltException>(engine.LastException);
    Assert.Equal(4, testInput.Items.Count);
    Assert.Equal(2, testOutput.Outputs.Count);
  }

  [Fact]
  public async Task ExceptionHandlingPostManualItemSingleItem()
  {
    var testInput = new TestInput();
    var testOutput = new TestOutput()
    {
      Outputs = new() { "PostException" }
    };
    var engine = GetEngineExceptionEngine<ItemHaltException>();
    await engine.ApplyAsync(testInput, testOutput);
    Assert.NotNull(engine.LastException);
    Assert.IsType<ItemHaltException>(engine.LastException);
    Assert.Equal(engine.PostRules.First(), engine.LastException.Rule);
    Assert.NotNull(engine.LastException.Context);
    Assert.NotNull(engine.LastException);
    Assert.IsType<ItemHaltException>(engine.LastException);
    Assert.Equal(4, testInput.Items.Count);
    Assert.Equal(2, testOutput.Outputs.Count);
  }

  private static IAsyncRulesEngine<TestInput, TestOutput> GetExceptionEngine(IExceptionHandler handler)
   => EngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                  .WithPreRule("testprerule")
                    .WithAction(async (c, i, t) =>
                    {
                      i.Items.Add("testprerule");
                      if (i.Items.Contains("PreException")) throw new Exception();
                      i.Items.Add("testprerule");
                    })
                  .EndRule()
                  .WithRule("testrule")
                    .WithAction(async (c, i, o, t) =>
                    {
                      i.Items.Add("testrule");
                      if (i.Items.Contains("Exception")) throw new Exception();
                      i.Items.Add("testrule2");
                    })
                  .EndRule()
                  .WithPostRule("testpostrule")
                    .WithAction(async (c, o, t) =>
                    {
                      o.Outputs.Add("testpostrule");
                      if (o.Outputs.Contains("PostException")) throw new Exception();
                      o.Outputs.Add("testpostrule2");
                    })
                  .EndRule()
                  .WithExceptionHandler(handler)
                  .Build();

  private static IAsyncRulesEngine<TestInput, TestOutput> GetEngineExceptionEngine<T>() where T : EngineException, new()
   => EngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithPreRule("testprerule")
                  .WithAction(async (c, i, t) =>
                  {
                    i.Items.Add("testprerule");
                    if (i.Items.Contains("PreException")) throw new T();
                    i.Items.Add("testprerule");
                  })
                .EndRule()
                .WithRule("testrule")
                  .WithAction(async (c, i, o, t) =>
                  {
                    i.Items.Add("testrule");
                    if (i.Items.Contains("Exception")) throw new T();
                    i.Items.Add("testrule2");
                  })
                .EndRule()
                .WithPostRule("testpostrule")
                  .WithAction(async (c, o, t) =>
                  {
                    o.Outputs.Add("testpostrule");
                    if (o.Outputs.Contains("PostException")) throw new T();
                    o.Outputs.Add("testpostrule2");
                  })
                .EndRule()
                .Build();
}
