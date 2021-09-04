using Rubric.Tests.TestRules;
using Rubric.Tests.TestRules.Async;
using Rubric;

namespace Rubric.Tests.Engines.Async;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
public class AsyncEngineOfTInTOutTests
{

  [Fact]
  public async Task Applies()
  {
    var rule = new TestDefaultAsyncRule();
    var input = new TestInput();
    var output = new TestOutput();
    var engine = new AsyncRuleEngine<TestInput, TestOutput>(
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
    var engine = new AsyncRuleEngine<TestInput, TestOutput>(
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
    var engine = new AsyncRuleEngine<TestInput, TestOutput>(
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
    var engine = new AsyncRuleEngine<TestInput, TestOutput>(ruleSet, false, null, logger);
    Assert.Equal(logger, engine.Logger);
    Assert.False(engine.IsParallel);
  }

  [Fact]
  public void ConstructorNullLogger()
  {
    var ruleSet = new AsyncRuleset<TestInput, TestOutput>();
    var engine = new AsyncRuleEngine<TestInput, TestOutput>(ruleSet);
    Assert.NotNull(engine.Logger);
  }

  [Fact]
  public void ConstructorParallel()
  {
    var ruleSet = new AsyncRuleset<TestInput, TestOutput>();
    var engine = new AsyncRuleEngine<TestInput, TestOutput>(ruleSet, true);
    Assert.True(engine.IsParallel);
  }

  [Fact]
  public void ConstructorWithEmptySyncRuleset()
  {
    var logger = new TestLogger();
    var ruleSet = new Ruleset<TestInput, TestOutput>();
    var engine = new AsyncRuleEngine<TestInput, TestOutput>(ruleSet, false, null, logger);
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
    var engine = new AsyncRuleEngine<TestInput, TestOutput>(ruleSet, false, null, logger);
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
  public async Task FullRunStreamHandleEngineException()
  {
    var engine = EngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                              .WithPostRule("test")
                                .WithAction((c, o) => throw new Exception())
                              .EndRule()
                              .WithExceptionHandler(ExceptionHandlers.HaltEngine)
                              .Build();
    var input1 = new TestInput();
    var input2 = new TestInput();
    var output = new TestOutput();
    await engine.ApplyAsync(new[] { input1, input2 }.ToAsyncEnumerable(), output);
    Assert.IsType<EngineHaltException>(engine.LastException);
  }

  [Fact]
  public async Task FullRunStream()
  {
    var engine = EngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                              .WithRule("test")
                                .WithAction((c, i, o, t) => { o.Counter++; return Task.CompletedTask; })
                              .EndRule()
                              .Build();
    var input1 = new TestInput();
    var input2 = new TestInput();
    var output = new TestOutput();
    await engine.ApplyAsync(new[] { input1, input2 }.ToAsyncEnumerable(), output);
    Assert.Equal(2, output.Counter);
  }

  [Fact]
  public async Task FullRunStreamAsParallel()
  {
    var engine = EngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                              .WithRule("test")
                                .WithAction((c, i, o, t) => { o.Counter++; return Task.CompletedTask; })
                              .EndRule()
                              .AsParallel()
                              .Build();
    var input1 = new TestInput();
    var input2 = new TestInput();
    var output = new TestOutput();
    await engine.ApplyAsync(new[] { input1, input2 }.ToAsyncEnumerable(), output);
    Assert.Equal(2, output.Counter);
  }

  [Fact]
  public async Task NotApplies()
  {
    var rule = new TestAsyncRule(false);
    var input = new TestInput();
    var output = new TestOutput();
    var engine = new AsyncRuleEngine<TestInput, TestOutput>(
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
    var engine = new AsyncRuleEngine<TestInput, TestOutput>(
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
    var engine = new AsyncRuleEngine<TestInput, TestOutput>(
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
    var engine = new AsyncRuleEngine<TestInput, TestOutput>(
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
    var engine = new AsyncRuleEngine<TestInput, TestOutput>(
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
    var engine = new AsyncRuleEngine<TestInput, TestOutput>(
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
    var engine = new AsyncRuleEngine<TestInput, TestOutput>(
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
        new AsyncRuleEngine<TestInput, TestOutput>(
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
        new AsyncRuleEngine<TestInput, TestOutput>(
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
        new AsyncRuleEngine<TestInput, TestOutput>(
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
        new AsyncRuleEngine<TestInput, TestOutput>(
            new AsyncRule<TestInput>[] { testPreRule }, null, null);
    var input = new TestInput();
    var output = new TestOutput();
    var exception = await Assert.ThrowsAsync<Exception>(async () => await engine.ApplyAsync(input, output));
    Assert.IsNotType<EngineException>(exception);
    Assert.Null(engine.LastException);
    Assert.True(input.InputFlag);
  }

  [Theory]
  [InlineData(false, false)]
  [InlineData(true, false)]
  [InlineData(false, true)]
  [InlineData(true, true)]
  public async Task ExceptionHandlingIgnore(bool parallelizeRules, bool parallelizeInputs)
  {
    var testInput = new TestInput
    {
      Items = new() { "PreException", "Exception" }
    };
    var testInput2 = new TestInput();
    var testOutput = new TestOutput
    {
      Outputs = new() { "PostException" }
    };
    var engine = GetExceptionEngine<Exception>(ExceptionHandlers.Ignore, parallelizeRules);
    if (parallelizeInputs)
      await engine.ApplyParallelAsync(new[] { testInput, testInput2 }, testOutput);
    else
      await engine.ApplyAsync(new[] { testInput, testInput2 }, testOutput);
    Assert.Null(engine.LastException);
    Assert.Equal(4, testInput.Items.Count);
    Assert.Equal(4, testInput2.Items.Count);
    Assert.Equal(2, testOutput.Outputs.Count);
  }

  [Theory]
  [InlineData(false, false)]
  [InlineData(true, false)]
  [InlineData(false, true)]
  [InlineData(true, true)]
  public async Task ExceptionHandlingPreThrow(bool parallelizeRules, bool parallelizeInputs)
  {
    var testInput = new TestInput
    {
      Items = new() { "PreException", "Exception" }
    };
    var testInput2 = new TestInput();
    var testOutput = new TestOutput();
    var engine = GetExceptionEngine<Exception>(ExceptionHandlers.Throw, parallelizeRules);
    if (parallelizeInputs)
      await Assert.ThrowsAsync<Exception>(() => engine.ApplyParallelAsync(new[] { testInput, testInput2 }, testOutput));
    else
      await Assert.ThrowsAsync<Exception>(() => engine.ApplyAsync(new[] { testInput, testInput2 }, testOutput));
    Assert.Null(engine.LastException);
    Assert.Equal(3, testInput.Items.Count);
    if (!parallelizeInputs)
      Assert.Empty(testInput2.Items);
    Assert.Empty(testOutput.Outputs);

  }

  [Theory]
  [InlineData(false, false)]
  [InlineData(true, false)]
  [InlineData(false, true)]
  [InlineData(true, true)]
  public async Task ExceptionHandlingPreItem(bool parallelizeRules, bool parallelizeInputs)
  {
    var testInput = new TestInput
    {
      Items = new() { "PreException", "Exception" }
    };
    var testInput2 = new TestInput();
    var testOutput = new TestOutput();
    var engine = GetExceptionEngine<Exception>(ExceptionHandlers.HaltItem, parallelizeRules);
    if (parallelizeInputs)
      await engine.ApplyParallelAsync(new[] { testInput, testInput2 }, testOutput);
    else
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

  [Theory]
  [InlineData(false, false)]
  [InlineData(true, false)]
  [InlineData(false, true)]
  [InlineData(true, true)]
  public async Task ExceptionHandlingPreEngine(bool parallelizeRules, bool parallelizeInputs)
  {
    var testInput = new TestInput
    {
      Items = new() { "PreException", "Exception" }
    };
    var testInput2 = new TestInput();
    var testOutput = new TestOutput();
    var engine = GetExceptionEngine<Exception>(ExceptionHandlers.HaltEngine, parallelizeRules);
    if (parallelizeInputs)
      await engine.ApplyParallelAsync(new[] { testInput, testInput2 }, testOutput);
    else
      await engine.ApplyAsync(new[] { testInput, testInput2 }, testOutput);
    Assert.NotNull(engine.LastException);
    Assert.IsType<EngineHaltException>(engine.LastException);
    Assert.Equal(testInput, engine.LastException.Input);
    Assert.Equal(engine.PreRules.First(), engine.LastException.Rule);
    Assert.NotNull(engine.LastException.Context);
    Assert.Equal(3, testInput.Items.Count);
    if (!parallelizeInputs)
      Assert.Empty(testInput2.Items);
    Assert.Empty(testOutput.Outputs);
  }

  [Theory]
  [InlineData(false, false)]
  [InlineData(true, false)]
  [InlineData(false, true)]
  [InlineData(true, true)]
  public async Task ExceptionHandlingPreHandlerException(bool parallelizeRules, bool parallelizeInputs)
  {
    var testInput = new TestInput
    {
      Items = new() { "PreException", "Exception" }
    };
    var testInput2 = new TestInput();
    var testOutput = new TestOutput();
    var engine = GetExceptionEngine<Exception>(new LambdaExceptionHandler(
      (e, c, i, o, r) => throw new InvalidOperationException()), parallelizeRules);
    if (parallelizeInputs)
      await Assert.ThrowsAsync<InvalidOperationException>(() => engine.ApplyParallelAsync(new[] { testInput, testInput2 }, testOutput));
    else
      await Assert.ThrowsAsync<InvalidOperationException>(() => engine.ApplyAsync(new[] { testInput, testInput2 }, testOutput));
    Assert.Null(engine.LastException);
    Assert.Equal(3, testInput.Items.Count);
    Assert.Empty(testOutput.Outputs);
  }

  [Theory]
  [InlineData(false, false)]
  [InlineData(true, false)]
  [InlineData(false, true)]
  [InlineData(true, true)]
  public async Task ExceptionHandlingHandlerException(bool parallelizeRules, bool parallelizeInputs)
  {
    var testInput = new TestInput
    {
      Items = new() { "Exception" }
    };
    var testInput2 = new TestInput();
    var testOutput = new TestOutput();
    var engine = GetExceptionEngine<Exception>(new LambdaExceptionHandler(
      (e, c, i, o, r) => throw new InvalidOperationException()), parallelizeRules);
    if (parallelizeInputs)
      await Assert.ThrowsAsync<InvalidOperationException>(() => engine.ApplyParallelAsync(new[] { testInput, testInput2 }, testOutput));
    else
      await Assert.ThrowsAsync<InvalidOperationException>(() => engine.ApplyAsync(new[] { testInput, testInput2 }, testOutput));
    Assert.Null(engine.LastException);
    Assert.Equal(4, testInput.Items.Count);
    if (!parallelizeInputs)
      Assert.Empty(testInput2.Items);
    Assert.Empty(testOutput.Outputs);
  }

  [Theory]
  [InlineData(false, false)]
  [InlineData(true, false)]
  [InlineData(false, true)]
  [InlineData(true, true)]
  public async Task ExceptionHandlingPreManualItem(bool parallelizeRules, bool parallelizeInputs)
  {
    var testInput = new TestInput
    {
      Items = new() { "PreException" }
    };
    var testInput2 = new TestInput();
    var testOutput = new TestOutput();
    var engine = GetEngineExceptionEngine<ItemHaltException>(parallelizeRules);
    if (parallelizeInputs)
      await engine.ApplyParallelAsync(new[] { testInput, testInput2 }, testOutput);
    else
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

  [Theory]
  [InlineData(false, false)]
  [InlineData(true, false)]
  [InlineData(false, true)]
  [InlineData(true, true)]
  public async Task ExceptionHandlingManualEngine(bool parallelizeRules, bool parallelizeInputs)
  {
    var testInput = new TestInput
    {
      Items = new() { "Exception" }
    };
    var testInput2 = new TestInput();
    var testOutput = new TestOutput();
    var engine = GetEngineExceptionEngine<EngineHaltException>(parallelizeRules);
    if (parallelizeInputs)
      await engine.ApplyParallelAsync(new[] { testInput, testInput2 }, testOutput);
    else
      await engine.ApplyAsync(new[] { testInput, testInput2 }, testOutput);
    Assert.NotNull(engine.LastException);
    Assert.IsType<EngineHaltException>(engine.LastException);
    Assert.Equal(testInput, engine.LastException.Input);
    Assert.Equal(engine.Rules.First(), engine.LastException.Rule);
    Assert.NotNull(engine.LastException.Context);
    Assert.Equal(4, testInput.Items.Count);
    if (parallelizeInputs)
      Assert.NotEmpty(testInput2.Items);
    else
      Assert.Empty(testInput2.Items);
    Assert.Empty(testOutput.Outputs);
  }

  [Theory]
  [InlineData(false, false)]
  [InlineData(true, false)]
  [InlineData(false, true)]
  [InlineData(true, true)]
  public async Task ExceptionHandlingItemFromForeignTaskCancelled(bool parallelizeRules, bool parallelizeInputs)
  {
    var testInput = new TestInput
    {
      Items = new() { "Exception" }
    };
    var testInput2 = new TestInput();
    var testOutput = new TestOutput();
    var engine = GetExceptionEngine<TaskCanceledException>(ExceptionHandlers.HaltItem, parallelizeRules);
    if (parallelizeInputs)
      await engine.ApplyParallelAsync(new[] { testInput, testInput2 }, testOutput, null, new CancellationTokenSource().Token);
    else
      await engine.ApplyAsync(new[] { testInput, testInput2 }, testOutput, null, new CancellationTokenSource().Token);
    Assert.NotNull(engine.LastException);
    Assert.IsType<ItemHaltException>(engine.LastException);
    Assert.Equal(testInput, engine.LastException.Input);
    Assert.Equal(engine.Rules.First(), engine.LastException.Rule);
    Assert.NotNull(engine.LastException.Context);
    Assert.Equal(4, testInput.Items.Count);
    Assert.NotEmpty(testInput2.Items);
    Assert.Equal(2, testOutput.Outputs.Count);
  }

  [Theory]
  [InlineData(false, false)]
  [InlineData(true, false)]
  [InlineData(false, true)]
  [InlineData(true, true)]
  public async Task ExceptionHandlingManualItem(bool parallelizeRules, bool parallelizeInputs)
  {
    var testInput = new TestInput
    {
      Items = new() { "Exception" }
    };
    var testInput2 = new TestInput();
    var testOutput = new TestOutput();
    var engine = GetEngineExceptionEngine<ItemHaltException>(parallelizeRules);
    if (parallelizeInputs)
      await engine.ApplyParallelAsync(new[] { testInput, testInput2 }, testOutput);
    else
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

  [Theory]
  [InlineData(false, false)]
  [InlineData(true, false)]
  [InlineData(false, true)]
  [InlineData(true, true)]
  public async Task ExceptionHandlingPreManualEngine(bool parallelizeRules, bool parallelizeInputs)
  {
    var testInput = new TestInput
    {
      Items = new() { "PreException" }
    };
    var testInput2 = new TestInput();
    var testOutput = new TestOutput();
    var engine = GetEngineExceptionEngine<EngineHaltException>(parallelizeRules);
    if (parallelizeInputs)
      await engine.ApplyParallelAsync(new[] { testInput, testInput2 }, testOutput);
    else
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
    //Should have *something* if executing in parallel
    if (!parallelizeInputs)
      Assert.Empty(testInput2.Items);
    Assert.Empty(testOutput.Outputs);
  }

  [Theory]
  [InlineData(false, false)]
  [InlineData(true, false)]
  [InlineData(false, true)]
  [InlineData(true, true)]
  public async Task ExceptionHandlingPostManualEngine(bool parallelizeRules, bool parallelizeInputs)
  {
    var testInput = new TestInput();
    var testInput2 = new TestInput();
    var testOutput = new TestOutput
    {
      Outputs = new() { "PostException" }
    };
    var engine = GetEngineExceptionEngine<EngineHaltException>(parallelizeRules);
    if (parallelizeInputs)
      await engine.ApplyParallelAsync(new[] { testInput, testInput2 }, testOutput);
    else
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

  [Theory]
  [InlineData(false, false)]
  [InlineData(true, false)]
  [InlineData(false, true)]
  [InlineData(true, true)]
  public async Task ExceptionHandlingPostManualItem(bool parallelizeRules, bool parallelizeInputs)
  {
    var testInput = new TestInput();
    var testInput2 = new TestInput();
    var testOutput = new TestOutput
    {
      Outputs = new() { "PostException" }
    };
    var engine = GetEngineExceptionEngine<ItemHaltException>(parallelizeRules);
    if (parallelizeInputs)
      await engine.ApplyParallelAsync(new[] { testInput, testInput2 }, testOutput);
    else
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

  [Theory]
  [InlineData(false, false)]
  [InlineData(true, false)]
  [InlineData(false, true)]
  [InlineData(true, true)]
  public async Task ExceptionHandlingPostManualEngineSingleItem(bool parallelizeRules, bool parallelizeInputs)
  {
    var testInput = new TestInput();
    var testOutput = new TestOutput
    {
      Outputs = new() { "PostException" }
    };
    var engine = GetEngineExceptionEngine<EngineHaltException>(parallelizeRules);
    if (parallelizeInputs)
      await engine.ApplyParallelAsync(new[] { testInput }, testOutput);
    else
      await engine.ApplyAsync(new[] { testInput }, testOutput);
    Assert.NotNull(engine.LastException);
    Assert.IsType<EngineHaltException>(engine.LastException);
    Assert.Equal(engine.PostRules.First(), engine.LastException.Rule);
    Assert.NotNull(engine.LastException.Context);
    Assert.NotNull(engine.LastException);
    Assert.IsType<EngineHaltException>(engine.LastException);
    Assert.Equal(4, testInput.Items.Count);
    Assert.Equal(2, testOutput.Outputs.Count);
  }

  [Theory]
  [InlineData(false, false)]
  [InlineData(true, false)]
  [InlineData(false, true)]
  [InlineData(true, true)]
  public async Task ExceptionHandlingPostManualItemSingleItem(bool parallelizeRules, bool parallelizeInputs)
  {
    var testInput = new TestInput();
    var testOutput = new TestOutput
    {
      Outputs = new() { "PostException" }
    };
    var engine = GetEngineExceptionEngine<ItemHaltException>(parallelizeRules);
    if (parallelizeInputs)
      await engine.ApplyParallelAsync(new[] { testInput }, testOutput);
    else
      await engine.ApplyAsync(new[] { testInput }, testOutput);
    Assert.NotNull(engine.LastException);
    Assert.IsType<ItemHaltException>(engine.LastException);
    Assert.Equal(engine.PostRules.First(), engine.LastException.Rule);
    Assert.NotNull(engine.LastException.Context);
    Assert.NotNull(engine.LastException);
    Assert.IsType<ItemHaltException>(engine.LastException);
    Assert.Equal(4, testInput.Items.Count);
    Assert.Equal(2, testOutput.Outputs.Count);
  }

  private static IAsyncRuleEngine<TestInput, TestOutput> GetExceptionEngine<T>(IExceptionHandler handler, bool parallelizeRules)
    where T : Exception, new()
  {
    var builder =
      EngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
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
                  .WithExceptionHandler(handler);
    if (parallelizeRules)
      builder.AsParallel();
    return builder.Build();
  }

  private static IAsyncRuleEngine<TestInput, TestOutput> GetEngineExceptionEngine<T>(bool parallelizeRules) where T : EngineException, new()
  {
    var builder =
      EngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
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
                  .EndRule();
    if (parallelizeRules)
    {
      builder.AsParallel();
    }
    return builder.Build();
  }

}