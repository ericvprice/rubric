using Rubric.Async;
using Rubric.Engines.Async;
using Rubric.Tests.TestRules.Async;

namespace Rubric.Tests.Engines.Async;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
public class ParallelAsyncEngineTests
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
    )
    {
      IsParallel = true
    };
    await engine.ApplyAsync(input, output);
    Assert.True(input.InputFlag);
    Assert.True(output.TestFlag);
  }

  [Fact]
  public async Task FullRun()
  {
    var engine = EngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                              .WithAsyncPreRule("test")
                              .WithPredicate((_, _) => Task.FromResult(true))
                              .WithAction((_, i) =>
                              {
                                i.Items.Add("pre");
                                return Task.CompletedTask;
                              })
                              .EndRule()
                              .WithAsyncRule("test")
                              .WithPredicate((_, _, _) => Task.FromResult(true))
                              .WithAction((_, i, o) =>
                              {
                                i.Items.Add("rule");
                                o.Outputs.Add("rule");
                                return Task.CompletedTask;
                              })
                              .EndRule()
                              .WithAsyncPostRule("test")
                              .WithPredicate((_, _) => Task.FromResult(true))
                              .WithAction((_, o) =>
                              {
                                o.Outputs.Add("postrule");
                                return Task.CompletedTask;
                              })
                              .EndRule()
                              .AsParallel()
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
    var engine = new AsyncRuleEngine<TestInput, TestOutput>(
        null,
        new IAsyncRule<TestInput, TestOutput>[] { rule },
        null
    )
    {
      IsParallel = true
    };
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
    )
    {
      IsParallel = true
    };
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
    )
    {
      IsParallel = true
    };
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
    )
    {
      IsParallel = true
    };
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
    )
    {
      IsParallel = true
    };
    await engine.ApplyAsync(input, output);
    Assert.False(input.InputFlag);
  }

  [Fact]
  public async Task WrapApplyAsyncException()
  {
    var engine = new AsyncRuleEngine<TestInput, TestOutput>(
        null,
        new AsyncRule<TestInput, TestOutput>[] { new TestExceptionAsyncRule(false) },
        null)
    {
      IsParallel = true
    };
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
  public async Task WrapDoesApplyAsyncException()
  {
    var engine = new AsyncRuleEngine<TestInput, TestOutput>(
        null,
        new AsyncRule<TestInput, TestOutput>[] { new TestExceptionAsyncRule(true) },
        null)
    {
      IsParallel = true
    };
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
  public async Task WrapPostApplyAsyncException()
  {
    var engine = new AsyncRuleEngine<TestInput, TestOutput>(
        null,
        null,
        new AsyncRule<TestOutput>[] { new TestExceptionAsyncPostRule(false) })
    {
      IsParallel = true
    };
    var input = new TestInput();
    var output = new TestOutput();
    var context = new EngineContext();
    var exception = await Assert.ThrowsAsync<Exception>(() => engine.ApplyAsync(input, output, context));
    Assert.IsNotType<EngineException>(exception);
    Assert.Null(context.GetLastException());
    Assert.True(output.TestFlag);
  }

  [Fact]
  public async Task WrapPostDoesApplyAsyncException()
  {
    var engine = new AsyncRuleEngine<TestInput, TestOutput>(
        null,
        null,
        new AsyncRule<TestOutput>[] { new TestExceptionAsyncPostRule(true) })
    {
      IsParallel = true
    };
    var input = new TestInput();
    var output = new TestOutput();
    var context = new EngineContext();
    var exception = await Assert.ThrowsAsync<Exception>(() => engine.ApplyAsync(input, output, context));
    Assert.IsNotType<EngineException>(exception);
    Assert.Null(context.GetLastException());
    Assert.False(output.TestFlag);
  }

  [Fact]
  public async Task WrapPreApplyAsyncException()
  {
    var engine = new AsyncRuleEngine<TestInput, TestOutput>(
        new AsyncRule<TestInput>[] { new TestExceptionAsyncPreRule(false) },
        null,
        null)
    {
      IsParallel = true
    };
    var input = new TestInput();
    var output = new TestOutput();
    var context = new EngineContext();
    var exception = await Assert.ThrowsAsync<Exception>(() => engine.ApplyAsync(input, output, context));
    Assert.IsNotType<EngineException>(exception);
    Assert.Null(context.GetLastException());
    Assert.True(input.InputFlag);
  }


  [Fact]
  public async Task WrapPreDoesApplyAsyncException()
  {
    var engine = new AsyncRuleEngine<TestInput, TestOutput>(
        new AsyncRule<TestInput>[] { new TestExceptionAsyncPreRule(true) },
        null,
        null)
    {
      IsParallel = true
    };
    var input = new TestInput();
    var output = new TestOutput();
    var context = new EngineContext();
    var exception = await Assert.ThrowsAsync<Exception>(async () => await engine.ApplyAsync(input, output, context));
    Assert.IsNotType<EngineException>(exception);
    Assert.Null(context.GetLastException());
    Assert.False(input.InputFlag);
  }

  [Fact]
  public async Task ExceptionHandlingIgnore()
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
    var engine = GetExceptionEngine(ExceptionHandlers.Ignore);
    var context = new EngineContext();
    await engine.ApplyAsync(new[] { testInput, testInput2 }, testOutput, context);
    Assert.Null(context.GetLastException());
    Assert.Equal(4, testInput.Items.Count);
    Assert.Equal(4, testInput2.Items.Count);
    Assert.Equal(2, testOutput.Outputs.Count);

  }

  [Fact]
  public async Task ExceptionHandlingPreThrow()
  {
    var testInput = new TestInput
    {
      Items = new() { "PreException", "Exception" }
    };
    var testInput2 = new TestInput();
    var testOutput = new TestOutput();
    var engine = GetExceptionEngine(ExceptionHandlers.Rethrow);
    var context = new EngineContext();
    await Assert.ThrowsAsync<Exception>(() => engine.ApplyAsync(new[] { testInput, testInput2 }, testOutput, context));
    Assert.Null(context.GetLastException());
    Assert.Equal(3, testInput.Items.Count);
    Assert.Empty(testInput2.Items);
    Assert.Empty(testOutput.Outputs);

  }

  [Fact]
  public async Task ExceptionHandlingPreItem()
  {
    var testInput = new TestInput
    {
      Items = new() { "PreException", "Exception" }
    };
    var testInput2 = new TestInput();
    var testOutput = new TestOutput();
    var context = new EngineContext();
    var engine = GetExceptionEngine(ExceptionHandlers.HaltItem);
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

  [Fact]
  public async Task ExceptionHandlingPreEngine()
  {
    var testInput = new TestInput
    {
      Items = new() { "PreException", "Exception" }
    };
    var testInput2 = new TestInput();
    var testOutput = new TestOutput();
    var context = new EngineContext();
    var engine = GetExceptionEngine(ExceptionHandlers.HaltEngine);
    await engine.ApplyAsync(new[] { testInput, testInput2 }, testOutput, context);
    var ex = context.GetLastException();
    Assert.NotNull(ex);
    Assert.IsType<EngineHaltException>(ex);
    Assert.Equal(testInput, ex.Input);
    Assert.Equal(engine.PreRules.First(), ex.Rule);
    Assert.NotNull(ex.Context);
    Assert.Equal(3, testInput.Items.Count);
    Assert.Empty(testInput2.Items);
    Assert.Empty(testOutput.Outputs);
  }

  [Fact]
  public async Task ExceptionHandlingPreHandlerException()
  {
    var testInput = new TestInput
    {
      Items = new() { "PreException", "Exception" }
    };
    var testInput2 = new TestInput();
    var testOutput = new TestOutput();
    var engine = GetExceptionEngine(new LambdaExceptionHandler(
      (_, _, _, _, _) => throw new InvalidOperationException()));
    var context = new EngineContext();
    await Assert.ThrowsAsync<InvalidOperationException>(() => engine.ApplyAsync(new[] { testInput, testInput2 }, testOutput));
    Assert.Null(context.GetLastException());
    Assert.Equal(3, testInput.Items.Count);
    Assert.Empty(testInput2.Items);
    Assert.Empty(testOutput.Outputs);
  }

  [Fact]
  public async Task ExceptionHandlingHandlerException()
  {
    var testInput = new TestInput
    {
      Items = new() { "Exception" }
    };
    var testInput2 = new TestInput();
    var testOutput = new TestOutput();
    var engine = GetExceptionEngine(new LambdaExceptionHandler(
      (_, _, _, _, _) => throw new InvalidOperationException()));
    var context = new EngineContext();
    await Assert.ThrowsAsync<InvalidOperationException>(() => engine.ApplyAsync(new[] { testInput, testInput2 }, testOutput, context));
    Assert.Null(context.GetLastException());
    Assert.Equal(4, testInput.Items.Count);
    Assert.Empty(testInput2.Items);
    Assert.Empty(testOutput.Outputs);
  }

  [Fact]
  public async Task ExceptionHandlingPreManualItem()
  {
    var testInput = new TestInput
    {
      Items = new() { "PreException" }
    };
    var testInput2 = new TestInput();
    var testOutput = new TestOutput();
    var engine = GetEngineExceptionEngine<ItemHaltException>();
    var context = new EngineContext();
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

  [Fact]
  public async Task ExceptionHandlingManualEngine()
  {
    var testInput = new TestInput
    {
      Items = new() { "Exception" }
    };
    var testInput2 = new TestInput();
    var testOutput = new TestOutput();
    var engine = GetEngineExceptionEngine<EngineHaltException>();
    var context = new EngineContext();
    await engine.ApplyAsync(new[] { testInput, testInput2 }, testOutput, context);
    var ex = context.GetLastException();
    Assert.NotNull(ex);
    Assert.IsType<EngineHaltException>(ex);
    Assert.Equal(testInput, ex.Input);
    Assert.Equal(engine.Rules.First(), ex.Rule);
    Assert.NotNull(ex.Context);
    Assert.Equal(4, testInput.Items.Count);
    Assert.Empty(testInput2.Items);
    Assert.Empty(testOutput.Outputs);
  }

  [Fact]
  public async Task ExceptionHandlingManualItem()
  {
    var testInput = new TestInput
    {
      Items = new() { "Exception" }
    };
    var testInput2 = new TestInput();
    var testOutput = new TestOutput();
    var engine = GetEngineExceptionEngine<ItemHaltException>();
    var context = new EngineContext();
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

  [Fact]
  public async Task ExceptionHandlingPreManualEngine()
  {
    var testInput = new TestInput
    {
      Items = new() { "PreException" }
    };
    var testInput2 = new TestInput();
    var testOutput = new TestOutput();
    var engine = GetEngineExceptionEngine<EngineHaltException>();
    var context = new EngineContext();
    await engine.ApplyAsync(new[] { testInput, testInput2 }, testOutput, context);
    var ex = context.GetLastException();
    Assert.NotNull(ex);
    Assert.IsType<EngineHaltException>(ex);
    Assert.Equal(testInput, ex.Input);
    Assert.Equal(engine.PreRules.First(), ex.Rule);
    Assert.NotNull(ex.Context);
    Assert.NotNull(ex);
    Assert.IsType<EngineHaltException>(ex);
    Assert.Equal(testInput, ex.Input);
    Assert.Equal(engine.PreRules.First(), ex.Rule);
    Assert.NotNull(ex.Context);
    Assert.Equal(2, testInput.Items.Count);
    Assert.Empty(testInput2.Items);
    Assert.Empty(testOutput.Outputs);
  }

  [Fact]
  public async Task ExceptionHandlingPostManualEngine()
  {
    var testInput = new TestInput();
    var testInput2 = new TestInput();
    var testOutput = new TestOutput
    {
      Outputs = new() { "PostException" }
    };
    var engine = GetEngineExceptionEngine<EngineHaltException>();
    var context = new EngineContext();
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

  [Fact]
  public async Task ExceptionHandlingPostManualItem()
  {
    var testInput = new TestInput();
    var testInput2 = new TestInput();
    var testOutput = new TestOutput
    {
      Outputs = new() { "PostException" }
    };
    var engine = GetEngineExceptionEngine<ItemHaltException>();
    var context = new EngineContext();
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

  [Fact]
  public async Task ExceptionHandlingPostManualEngineSingleItem()
  {
    var testInput = new TestInput();
    var testOutput = new TestOutput
    {
      Outputs = new() { "PostException" }
    };
    var engine = GetEngineExceptionEngine<EngineHaltException>();
    var context = new EngineContext();
    await engine.ApplyAsync(testInput, testOutput, context);
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

  [Fact]
  public async Task ExceptionHandlingPostManualItemSingleItem()
  {
    var testInput = new TestInput();
    var testOutput = new TestOutput
    {
      Outputs = new() { "PostException" }
    };
    var engine = GetEngineExceptionEngine<ItemHaltException>();
    var context = new EngineContext();
    await engine.ApplyAsync(testInput, testOutput, context);
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

  private static IAsyncRuleEngine<TestInput, TestOutput> GetExceptionEngine(IExceptionHandler handler)
   => EngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                  .WithAsyncPreRule("testprerule")
                    .WithAction(async (_, i, _) =>
                    {
                      i.Items.Add("testprerule");
                      if (i.Items.Contains("PreException")) throw new();
                      i.Items.Add("testprerule");
                    })
                  .EndRule()
                  .WithAsyncRule("testrule")
                    .WithAction(async (_, i, _, _) =>
                    {
                      i.Items.Add("testrule");
                      if (i.Items.Contains("Exception")) throw new();
                      i.Items.Add("testrule2");
                    })
                  .EndRule()
                  .WithAsyncPostRule("testpostrule")
                    .WithAction(async (_, o, _) =>
                    {
                      o.Outputs.Add("testpostrule");
                      if (o.Outputs.Contains("PostException")) throw new();
                      o.Outputs.Add("testpostrule2");
                    })
                  .EndRule()
                  .WithExceptionHandler(handler)
                  .AsParallel()
                  .Build();

  private static IAsyncRuleEngine<TestInput, TestOutput> GetEngineExceptionEngine<T>() where T : EngineException, new()
   => EngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithAsyncPreRule("testprerule")
                  .WithAction(async (_, i, _) =>
                  {
                    i.Items.Add("testprerule");
                    if (i.Items.Contains("PreException")) throw new T();
                    i.Items.Add("testprerule");
                  })
                .EndRule()
                .WithAsyncRule("testrule")
                  .WithAction(async (_, i, _, _) =>
                  {
                    i.Items.Add("testrule");
                    if (i.Items.Contains("Exception")) throw new T();
                    i.Items.Add("testrule2");
                  })
                .EndRule()
                .WithAsyncPostRule("testpostrule")
                  .WithAction(async (_, o, _) =>
                  {
                    o.Outputs.Add("testpostrule");
                    if (o.Outputs.Contains("PostException")) throw new T();
                    o.Outputs.Add("testpostrule2");
                  })
                .EndRule()
                .AsParallel()
                .Build();
}
