using Rubric.Engines.Probabilistic.Async;
using Rubric.Engines.Probabilistic.Async.Implementation;
using Rubric.Rules.Probabilistic.Async;
using Rubric.Rulesets.Probabilistic.Async;
using Rubric.Tests.TestRules.Probabilistic.Async;

namespace Rubric.Tests.Engines.Probabilistic.Async;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
public class EngineOfTInTOutTests
{
  [Fact]
  public void Properties()
  {
    var logger = new TestLogger();
    var ruleSet = new Ruleset<TestInput, TestOutput>();
    var engine = new RuleEngine<TestInput, TestOutput>(ruleSet, false, null, logger);
    Assert.True(engine.IsAsync);
    Assert.False(engine.IsParallel);
    Assert.Equal(typeof(TestInput), engine.InputType);
    Assert.Equal(typeof(TestOutput), engine.OutputType);
  }

  [Fact]
  public async Task Applies()
  {
    var rule = new TestDefaultRule();
    var input = new TestInput();
    var output = new TestOutput();
    var engine = new RuleEngine<TestInput, TestOutput>(
        null,
        new [] { rule },
        null
    );
    await engine.ApplyAsync(input, output);
    Assert.True(input.InputFlag);
    Assert.True(output.TestFlag);
  }

  [Fact]
  public async Task AppliesOrder()
  {
    var rule = new TestDefaultPreRule();
    var rule2 = new TestPreRule(1D, false);
    var input = new TestInput();
    var output = new TestOutput();
    var engine = new RuleEngine<TestInput, TestOutput>(
        new IRule<TestInput>[] { rule, rule2 },
        null,
        null
    );
    await engine.ApplyAsync(input, output);
    Assert.False(input.InputFlag);
  }

  [Fact]
  public async Task AppliesOrderReverse()
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
    await engine.ApplyAsync(input, output);
    Assert.True(input.InputFlag);
  }

  [Fact]
  public void Constructor()
  {
    var logger = new TestLogger();
    var ruleSet = new Ruleset<TestInput, TestOutput>();
    var engine = new RuleEngine<TestInput, TestOutput>(ruleSet, false, null, logger);
    Assert.Equal(logger, engine.Logger);
    Assert.False(engine.IsParallel);
  }

  [Fact]
  public void ConstructorNullLogger()
  {
    var ruleSet = new Ruleset<TestInput, TestOutput>();
    var engine = new RuleEngine<TestInput, TestOutput>(ruleSet);
    Assert.NotNull(engine.Logger);
  }

  [Fact]
  public void ConstructorParallel()
  {
    var ruleSet = new Ruleset<TestInput, TestOutput>();
    var engine = new RuleEngine<TestInput, TestOutput>(ruleSet, true);
    Assert.True(engine.IsParallel);
  }

  [Fact]
  public void ConstructorWithEmptySyncRuleset()
  {
    var logger = new TestLogger();
    var ruleSet = new Ruleset<TestInput, TestOutput>();
    var engine = new RuleEngine<TestInput, TestOutput>(ruleSet, false, null, logger);
    Assert.Equal(logger, engine.Logger);
  }

  [Fact]
  public void ConstructorWithSyncRuleset()
  {
    var logger = new TestLogger();
    var ruleSet = new Ruleset<TestInput, TestOutput>();
    ruleSet.AddPreRule(new TestPreRule(1D));
    ruleSet.AddPostRule(new TestPostRule(1D));
    ruleSet.AddRule(new TestRule(1D));
    var engine = new RuleEngine<TestInput, TestOutput>(ruleSet, false, null, logger);
    Assert.NotEmpty(engine.PreRules);
    Assert.NotEmpty(engine.Rules);
    Assert.NotEmpty(engine.PostRules);
  }

  [Theory]
  [InlineData(false, false)]
  [InlineData(false, true)]
  [InlineData(true, false)]
  [InlineData(true, true)]
  public async Task FullRun(bool parallelizeRules, bool parallelizeInputs)
  {
    var builder = ProbabilisticEngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                              .WithPreRule("test")
                                .WithPredicate((_, _) => Task.FromResult(1D))
                                .WithAction((_, i) =>
                                {
                                  i.Items.Add("pre");
                                  return Task.CompletedTask;
                                })
                              .EndRule()
                              .WithRule("test")
                                .WithPredicate((_, _, _) => Task.FromResult(1D))
                                .WithAction((_, i, o) =>
                                {
                                  i.Items.Add("rule");
                                  o.Outputs.Add("rule");
                                  return Task.CompletedTask;
                                })
                              .EndRule()
                              .WithPostRule("test")
                                .WithPredicate((_, _) => Task.FromResult(1D))
                                .WithAction((_, o) =>
                                {
                                  o.Outputs.Add("postrule");
                                  return Task.CompletedTask;
                                })
                              .EndRule();
    if (parallelizeRules)
    {
      builder.AsParallel();
    }
    var engine = builder.Build();
    var input1 = new TestInput();
    var input2 = new TestInput();
    var output = new TestOutput();
    if (parallelizeInputs)
      await engine.ApplyParallelAsync(new[] { input1, input2 }, output);
    else
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
    var engine = ProbabilisticEngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                              .WithPostRule("test")
                                .WithAction((_, _) => throw new())
                              .EndRule()
                              .WithExceptionHandler(ExceptionHandlers.HaltEngine)
                              .Build();
    var input1 = new TestInput();
    var input2 = new TestInput();
    var output = new TestOutput();
    var context = new EngineContext();
    await engine.ApplyAsync(new[] { input1, input2 }.ToAsyncEnumerable(), output, context);
    Assert.IsType<EngineHaltException>(context.GetLastException());
  }

  [Fact]
  public async Task FullRunStream()
  {
    var engine = ProbabilisticEngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                              .WithRule("test")
                                .WithAction((_, _, o, _) => { o.Counter++; return Task.CompletedTask; })
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
    var engine = ProbabilisticEngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                              .WithRule("test")
                                .WithAction((_, _, o, _) => { o.Counter++; return Task.CompletedTask; })
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
    var rule = new TestRule(0D);
    var input = new TestInput();
    var output = new TestOutput();
    var engine = new RuleEngine<TestInput, TestOutput>(
        null,
        new IRule<TestInput, TestOutput>[] { rule },
        null
    );
    await engine.ApplyAsync(input, output);
    Assert.False(input.InputFlag);
    Assert.False(output.TestFlag);
  }

  [Fact]
  public async Task PostApplies()
  {
    var rule = new TestDefaultPostRule();
    var input = new TestInput();
    var output = new TestOutput();
    var engine = new RuleEngine<TestInput, TestOutput>(
        null,
        null,
        new IRule<TestOutput>[] { rule }
    );
    await engine.ApplyAsync(input, output);
    Assert.True(output.TestFlag);
  }

  [Fact]
  public async Task PostNotApplies()
  {
    var rule = new TestPostRule(0D);
    var input = new TestInput();
    var output = new TestOutput();
    var engine = new RuleEngine<TestInput, TestOutput>(
        null,
        null,
        new IRule<TestOutput>[] { rule }
    );
    await engine.ApplyAsync(input, output);
    Assert.False(output.TestFlag);
  }

  [Fact]
  public async Task PreApplies()
  {
    var rule = new TestDefaultPreRule();
    var input = new TestInput();
    var output = new TestOutput();
    var engine = new RuleEngine<TestInput, TestOutput>(
        new IRule<TestInput>[] { rule },
        null,
        null
    );
    await engine.ApplyAsync(input, output);
    Assert.True(input.InputFlag);
  }

  [Fact]
  public async Task PreNotApplies()
  {
    var rule = new TestPreRule(0D);
    var input = new TestInput();
    var output = new TestOutput();
    var engine = new RuleEngine<TestInput, TestOutput>(
        new IRule<TestInput>[] { rule },
        null,
        null
    );
    await engine.ApplyAsync(input, output);
    Assert.False(input.InputFlag);
  }

  [Fact]
  public async Task ApplyAsyncException()
  {
    var engine = new RuleEngine<TestInput, TestOutput>(
        null, new Rule<TestInput, TestOutput>[] { new TestExceptionRule(false) }, null);
    var input = new TestInput();
    var output = new TestOutput();
    var context = new EngineContext();
    var exception = await Assert.ThrowsAsync<Exception>(() => engine.ApplyAsync(input, output, context));
    Assert.IsNotType<EngineException>(exception);
    Assert.Null(context.GetLastException());
    Assert.True(input.InputFlag);
    Assert.True(output.TestFlag);
  }

  [Fact]
  public async Task DoesApplyAsyncException()
  {
    var engine = new RuleEngine<TestInput, TestOutput>(
        null, new Rule<TestInput, TestOutput>[] { new TestExceptionRule(true) }, null);
    var input = new TestInput();
    var output = new TestOutput();
    var context = new EngineContext();
    var exception = await Assert.ThrowsAsync<Exception>(() => engine.ApplyAsync(input, output, context));
    Assert.IsNotType<EngineException>(exception);
    Assert.Null(context.GetLastException());
    Assert.False(input.InputFlag);
    Assert.False(output.TestFlag);
  }

  [Fact]
  public async Task PostApplyAsyncException()
  {
    var testPostRule = new TestExceptionPostRule(false);
    var engine =
        new RuleEngine<TestInput, TestOutput>(
            null, null, new Rule<TestOutput>[] { testPostRule });
    var input = new TestInput();
    var output = new TestOutput();
    var context = new EngineContext();
    var exception = await Assert.ThrowsAsync<Exception>(() => engine.ApplyAsync(input, output, context));
    Assert.True(output.TestFlag);
    Assert.IsNotType<EngineException>(exception);
    Assert.Null(context.GetLastException());
  }

  [Fact]
  public async Task PostDoesApplyAsyncException()
  {
    var testPostRule = new TestExceptionPostRule(false);
    var engine =
        new RuleEngine<TestInput, TestOutput>(
            null, null, new Rule<TestOutput>[] { testPostRule });
    var input = new TestInput();
    var output = new TestOutput();
    var context = new EngineContext();
    var exception = await Assert.ThrowsAsync<Exception>(() => engine.ApplyAsync(input, output, context));
    Assert.IsNotType<EngineException>(exception);
    Assert.Null(context.GetLastException());
    Assert.True(output.TestFlag);
  }

  [Fact]
  public async Task PreApplyAsyncException()
  {
    var testPreRule = new TestExceptionPreRule(false);
    var engine =
        new RuleEngine<TestInput, TestOutput>(
            new Rule<TestInput>[] { testPreRule }, null, null);
    var input = new TestInput();
    var output = new TestOutput();
    var context = new EngineContext();
    var exception = await Assert.ThrowsAsync<Exception>(() => engine.ApplyAsync(input, output, context));
    Assert.IsNotType<EngineException>(exception);
    Assert.Null(context.GetLastException());
    Assert.True(input.InputFlag);
  }

  [Fact]
  public async Task PreDoesApplyAsyncException()
  {
    var testPreRule = new TestExceptionPreRule(false);
    var engine =
        new RuleEngine<TestInput, TestOutput>(
            new Rule<TestInput>[] { testPreRule }, null, null);
    var input = new TestInput();
    var output = new TestOutput();
    var context = new EngineContext();
    var exception = await Assert.ThrowsAsync<Exception>(async () => await engine.ApplyAsync(input, output, context));
    Assert.IsNotType<EngineException>(exception);
    Assert.Null(context.GetLastException());
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
    var context = new EngineContext();
    if (parallelizeInputs)
      await engine.ApplyParallelAsync(new[] { testInput, testInput2 }, testOutput, context);
    else
      await engine.ApplyAsync(new[] { testInput, testInput2 }, testOutput, context);
    Assert.Null(context.GetLastException());
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
    var engine = GetExceptionEngine<Exception>(ExceptionHandlers.Rethrow, parallelizeRules);
    var context = new EngineContext();
    if (parallelizeInputs)
      await Assert.ThrowsAsync<Exception>(() => engine.ApplyParallelAsync(new[] { testInput, testInput2 }, testOutput, context));
    else
      await Assert.ThrowsAsync<Exception>(() => engine.ApplyAsync(new[] { testInput, testInput2 }, testOutput, context));
    Assert.Null(context.GetLastException());
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
    var context = new EngineContext();
    if (parallelizeInputs)
      await engine.ApplyParallelAsync(new[] { testInput, testInput2 }, testOutput, context);
    else
      await engine.ApplyAsync(new[] { testInput, testInput2 }, testOutput, context);
    var ex = context.GetLastException();
    Assert.NotNull(ex);
    Assert.IsType<ItemHaltException>(ex);
    Assert.Equal(testInput, ex.Input);
    Assert.Equal(engine.PreRules.First(), ex.Rule);
    Assert.NotNull(ex.Context);
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
    var context = new EngineContext();
    if (parallelizeInputs)
      await engine.ApplyParallelAsync(new[] { testInput, testInput2 }, testOutput, context);
    else
      await engine.ApplyAsync(new[] { testInput, testInput2 }, testOutput, context);
    var ex = context.GetLastException();
    Assert.NotNull(ex);
    Assert.IsType<EngineHaltException>(ex);
    Assert.Equal(testInput, ex.Input);
    Assert.Equal(engine.PreRules.First(), ex.Rule);
    Assert.NotNull(ex.Context);
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
      (_, _, _, _, _) => throw new InvalidOperationException()), parallelizeRules);
    var context = new EngineContext();
    if (parallelizeInputs)
      await Assert.ThrowsAsync<InvalidOperationException>(() => engine.ApplyParallelAsync(new[] { testInput, testInput2 }, testOutput, context));
    else
      await Assert.ThrowsAsync<InvalidOperationException>(() => engine.ApplyAsync(new[] { testInput, testInput2 }, testOutput, context));
    Assert.Null(context.GetLastException());
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
      (_, _, _, _, _) => throw new InvalidOperationException()), parallelizeRules);
    var context = new EngineContext();
    if (parallelizeInputs)
      await Assert.ThrowsAsync<InvalidOperationException>(() => engine.ApplyParallelAsync(new[] { testInput, testInput2 }, testOutput, context));
    else
      await Assert.ThrowsAsync<InvalidOperationException>(() => engine.ApplyAsync(new[] { testInput, testInput2 }, testOutput, context));
    Assert.Null(context.GetLastException());
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
    var context = new EngineContext();
    if (parallelizeInputs)
      await engine.ApplyParallelAsync(new[] { testInput, testInput2 }, testOutput, context);
    else
      await engine.ApplyAsync(new[] { testInput, testInput2 }, testOutput, context);
    var ex = context.GetLastException();
    Assert.NotNull(ex);
    Assert.IsType<ItemHaltException>(ex);
    Assert.Equal(testInput, ex.Input);
    Assert.Equal(engine.PreRules.First(), ex.Rule);
    Assert.NotNull(ex.Context);
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
  public async Task ExceptionHandlingItemFromForeignTaskCancelled(bool parallelizeRules, bool parallelizeInputs)
  {
    var testInput = new TestInput
    {
      Items = new() { "Exception" }
    };
    var testInput2 = new TestInput();
    var testOutput = new TestOutput();
    var engine = GetExceptionEngine<TaskCanceledException>(ExceptionHandlers.HaltItem, parallelizeRules);
    var context = new EngineContext();
    if (parallelizeInputs)
      await engine.ApplyParallelAsync(new[] { testInput, testInput2 }, testOutput, context, new CancellationTokenSource().Token);
    else
      await engine.ApplyAsync(new[] { testInput, testInput2 }, testOutput, context, new CancellationTokenSource().Token);
    var ex = context.GetLastException();
    Assert.NotNull(ex);
    Assert.IsType<ItemHaltException>(ex);
    Assert.Equal(testInput, ex.Input);
    Assert.Equal(engine.Rules.First(), ex.Rule);
    Assert.NotNull(ex.Context);
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
    var context = new EngineContext();
    if (parallelizeInputs)
      await engine.ApplyParallelAsync(new[] { testInput, testInput2 }, testOutput, context);
    else
      await engine.ApplyAsync(new[] { testInput, testInput2 }, testOutput, context);
    var ex = context.GetLastException();
    Assert.NotNull(ex);
    Assert.IsType<ItemHaltException>(ex);
    Assert.Equal(testInput, ex.Input);
    Assert.Equal(engine.Rules.First(), ex.Rule);
    Assert.NotNull(ex.Context);
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
    var context = new EngineContext();
    if (parallelizeInputs)
      await engine.ApplyParallelAsync(new[] { testInput, testInput2 }, testOutput, context);
    else
      await engine.ApplyAsync(new[] { testInput, testInput2 }, testOutput, context);
    var ex = context.GetLastException();
    Assert.NotNull(ex);
    Assert.IsType<EngineHaltException>(ex);
    Assert.Equal(testInput, ex.Input);
    Assert.Equal(engine.PreRules.First(), ex.Rule);
    Assert.NotNull(ex.Context);
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
    var context = new EngineContext();
    if (parallelizeInputs)
      await engine.ApplyParallelAsync(new[] { testInput, testInput2 }, testOutput, context);
    else
      await engine.ApplyAsync(new[] { testInput, testInput2 }, testOutput, context);
    var ex = context.GetLastException();
    Assert.NotNull(ex);
    Assert.IsType<EngineHaltException>(ex);
    Assert.Equal(engine.PostRules.First(), ex.Rule);
    Assert.NotNull(ex.Context);
    Assert.NotNull(ex);
    Assert.IsType<EngineHaltException>(ex);
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
    var context = new EngineContext();
    if (parallelizeInputs)
      await engine.ApplyParallelAsync(new[] { testInput, testInput2 }, testOutput, context);
    else
      await engine.ApplyAsync(new[] { testInput, testInput2 }, testOutput, context);
    var ex = context.GetLastException();
    Assert.NotNull(ex);
    Assert.IsType<ItemHaltException>(ex);
    Assert.Equal(engine.PostRules.First(), ex.Rule);
    Assert.NotNull(ex.Context);
    Assert.NotNull(ex);
    Assert.IsType<ItemHaltException>(ex);
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
    var context = new EngineContext();
    if (parallelizeInputs)
      await engine.ApplyParallelAsync(new[] { testInput }, testOutput, context);
    else
      await engine.ApplyAsync(new[] { testInput }, testOutput, context);
    var ex = context.GetLastException();
    Assert.NotNull(ex);
    Assert.IsType<EngineHaltException>(ex);
    Assert.Equal(engine.PostRules.First(), ex.Rule);
    Assert.NotNull(ex.Context);
    Assert.NotNull(ex);
    Assert.IsType<EngineHaltException>(ex);
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
    var context = new EngineContext();
    if (parallelizeInputs)
      await engine.ApplyParallelAsync(new[] { testInput }, testOutput, context);
    else
      await engine.ApplyAsync(new[] { testInput }, testOutput, context);
    var ex = context.GetLastException();
    Assert.NotNull(ex);
    Assert.IsType<ItemHaltException>(ex);
    Assert.Equal(engine.PostRules.First(), ex.Rule);
    Assert.NotNull(ex.Context);
    Assert.NotNull(ex);
    Assert.IsType<ItemHaltException>(ex);
    Assert.Equal(4, testInput.Items.Count);
    Assert.Equal(2, testOutput.Outputs.Count);
  }

  private static IRuleEngine<TestInput, TestOutput> GetExceptionEngine<T>(IExceptionHandler handler, bool parallelizeRules)
    where T : Exception, new()
  {
    var builder =
      ProbabilisticEngineBuilder
                  .ForInputAndOutputAsync<TestInput, TestOutput>()
                  .WithPreRule("testprerule")
                    .WithAction(async (_, i, _) =>
                    {
                      i.Items.Add("testprerule");
                      if (i.Items.Contains("PreException")) throw new T();
                      i.Items.Add("testprerule");
                    })
                  .EndRule()
                  .WithRule("testrule")
                    .WithAction(async (_, i, _, _) =>
                    {
                      i.Items.Add("testrule");
                      if (i.Items.Contains("Exception")) throw new T();
                      i.Items.Add("testrule2");
                    })
                  .EndRule()
                  .WithPostRule("testpostrule")
                    .WithAction(async (_, o, _) =>
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

  private static IRuleEngine<TestInput, TestOutput> GetEngineExceptionEngine<T>(bool parallelizeRules) where T : EngineException, new()
  {
    var builder = ProbabilisticEngineBuilder
                  .ForInputAndOutputAsync<TestInput, TestOutput>()
                  .WithPreRule("testprerule")
                    .WithAction(async (_, i, _) =>
                    {
                      i.Items.Add("testprerule");
                      if (i.Items.Contains("PreException")) throw new T();
                      i.Items.Add("testprerule");
                    })
                  .EndRule()
                  .WithRule("testrule")
                    .WithAction(async (_, i, _, _) =>
                    {
                      i.Items.Add("testrule");
                      if (i.Items.Contains("Exception")) throw new T();
                      i.Items.Add("testrule2");
                    })
                  .EndRule()
                  .WithPostRule("testpostrule")
                    .WithAction(async (_, o, _) =>
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
