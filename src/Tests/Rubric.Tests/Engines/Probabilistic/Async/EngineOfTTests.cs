using System.Diagnostics;
using Rubric.Engines.Probabilistic.Async.Implementation;
using Rubric.Rulesets.Probabilistic.Async;
using Rubric.Rules.Probabilistic.Async;
using Rubric.Tests.TestRules.Probabilistic.Async;

namespace Rubric.Tests.Engines.Probabilistic.Async;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
public class EngineOfTTests
{

  [Fact]
  public async Task AppliesOrder()
  {
    var rule = new TestDefaultPreRule();
    var rule2 = new TestPreRule(1D, false);
    var input = new TestInput();
    var engine = new RuleEngine<TestInput>(
        new IRule<TestInput>[] { rule, rule2 }
    );
    await engine.ApplyAsync(input);
    Assert.False(input.InputFlag);
  }

  [Fact]
  public async Task AppliesOrderReverse()
  {
    var rule = new TestDefaultPreRule();
    var rule2 = new TestPreRule(1D, false);
    var input = new TestInput { InputFlag = true };
    var engine = new RuleEngine<TestInput>(
        new IRule<TestInput>[] { rule2, rule }
    );
    await engine.ApplyAsync(input);
    Assert.True(input.InputFlag);
  }

  [Fact]
  public void Constructor()
  {
    var logger = new TestLogger();
    var ruleSet = new Ruleset<TestInput>();
    var engine = new RuleEngine<TestInput>(ruleSet, false, null, logger);
    Assert.Equal(logger, engine.Logger);
    Assert.False(engine.IsParallel);
  }

  [Fact]
  public void Properties()
  {
    var logger = new TestLogger();
    var ruleSet = new Ruleset<TestInput>();
    var engine = new RuleEngine<TestInput>(ruleSet, false, null, logger);
    Assert.True(engine.IsAsync);
    Assert.False(engine.IsParallel);
    Assert.Equal(typeof(TestInput), engine.InputType);
    Assert.Equal(typeof(TestInput), engine.OutputType);
  }

  [Fact]
  public void PropertiesParallel()
  {
    var logger = new TestLogger();
    var ruleSet = new Ruleset<TestInput>();
    var engine = new RuleEngine<TestInput>(ruleSet, true, null, logger);
    Assert.True(engine.IsAsync);
    Assert.True(engine.IsParallel);
    Assert.Equal(typeof(TestInput), engine.InputType);
  }

  [Fact]
  public void ConstructorNullLogger()
  {
    var ruleSet = new Ruleset<TestInput>();
    var engine = new RuleEngine<TestInput>(ruleSet);
    Assert.NotNull(engine.Logger);
  }

  [Fact]
  public void ConstructorParallel()
  {
    var ruleSet = new Ruleset<TestInput>();
    var engine = new RuleEngine<TestInput>(ruleSet, true);
    Assert.True(engine.IsParallel);
  }

  [Fact]
  public void ConstructorWithEmptySyncRuleset()
  {
    var logger = new TestLogger();
    var ruleSet = new Ruleset<TestInput>();
    var engine = new RuleEngine<TestInput>(ruleSet, false, null, logger);
    Assert.Equal(logger, engine.Logger);
  }

  [Fact]
  public void ConstructorWithSyncRuleset()
  {
    var logger = new TestLogger();
    var ruleSet = new Ruleset<TestInput>();
    ruleSet.AddRule(new TestPreRule(1D));
    var engine = new RuleEngine<TestInput>(ruleSet, false, null, logger);
    Assert.NotEmpty(engine.Rules);
  }

  [Fact]
  public void ConstructorWithEmptyRules()
  {
    var logger = new TestLogger();
    var engine = new RuleEngine<TestInput>((IEnumerable<Rule<TestInput>>)null, false, null, logger);
    Assert.Empty(engine.Rules);
  }


  [Fact]
  public async Task Applies()
  {
    var rule = new TestDefaultPreRule();
    var input = new TestInput();
    var engine = new RuleEngine<TestInput>(
        new IRule<TestInput>[] { rule }
    );
    await engine.ApplyAsync(input);
    Assert.True(input.InputFlag);
  }

  [Fact]
  public async Task AppliesMany()
  {
    var rule = new TestDefaultPreRule();
    var input = new TestInput();
    var input2 = new TestInput();
    var engine = new RuleEngine<TestInput>(
        new IRule<TestInput>[] { rule }
    );
    await engine.ApplyAsync(new[] { input, input2 });
    Assert.True(input.InputFlag);
    Assert.True(input2.InputFlag);
  }

  [Fact]
  public async Task AppliesManyEmpty()
  {
    var rule = new TestDefaultPreRule();
    var engine = new RuleEngine<TestInput>(
        new IRule<TestInput>[] { rule }
    );
    await engine.ApplyAsync(Array.Empty<TestInput>());
    //Shouldn't throw
  }

  [Fact]
  public async Task NotApplies()
  {
    var rule = new TestPreRule(0D);
    var input = new TestInput();
    var engine = new RuleEngine<TestInput>(
        new IRule<TestInput>[] { rule }
    );
    await engine.ApplyAsync(input);
    Assert.False(input.InputFlag);
  }

  [Fact]
  public async Task DoesApplyAsyncException()
  {
    var testPreRule = new TestExceptionPreRule(false);
    var engine =
        new RuleEngine<TestInput>(
            new [] { testPreRule });
    var input = new TestInput();
    var context = new EngineContext();
    var exception = await Assert.ThrowsAsync<Exception>(async () => await engine.ApplyAsync(input, context));
    Assert.IsNotType<EngineException>(exception);
    Assert.Null(context.GetLastException());
    Assert.True(input.InputFlag);
  }

  [Fact]
  public async Task TestEngineHalt()
  {
    var engine =
        new RuleEngine<TestInput>(
            new IRule<TestInput>[] { new TestPreHaltRule(), new TestPreRule(1D) });
    var input = new TestInput();
    await engine.ApplyAsync(input);
    //Engine should have halted on first rule, second rule not run
    Assert.False(input.InputFlag);
  }

  [Fact]
  public async Task TestHaltInDoesApply()
  {
    var engine = ProbabilisticEngineBuilder.ForInputAsync<TestInput>()
                              .WithRule("rule1")
                                .WithPredicate((_, _) => throw new EngineHaltException())
                                .WithAction((_, i) => { i.Items.Add("rule1"); return Task.CompletedTask; })
                              .EndRule()
                              .WithRule("rule2")
                                .WithPredicate((_, _) => Task.FromResult(1D))
                                .WithAction((_, i) => { i.Items.Add("rule2"); return Task.CompletedTask; })
                              .EndRule()
                              .Build();
    var input = new TestInput();
    await engine.ApplyAsync(input);
    //Engine was halted in first does apply rule.
    //Nothing should have modified the input.
    Assert.Empty(input.Items);

  }

  [Fact]
  public async Task TestHaltInApply()
  {
    var engine = ProbabilisticEngineBuilder.ForInputAsync<TestInput>()
                              .WithRule("rule1")
                                .WithPredicate((_, _) => Task.FromResult(1D))
                                .WithAction((_, _) => throw new EngineHaltException())
                              .EndRule()
                              .WithRule("rule2")
                                .WithPredicate((_, _) => Task.FromResult(1D))
                                .WithAction((_, i) => { i.Items.Add("rule2"); return Task.CompletedTask; })
                              .EndRule()
                              .Build();
    var input = new TestInput();
    await engine.ApplyAsync(input);
    //Engine was halted in first rule.
    //Nothing should have modified the input.
    Assert.Empty(input.Items);
  }

  [Fact]
  public async Task TestHaltCancellationInDoesApply()
  {
    var engine = ProbabilisticEngineBuilder.ForInputAsync<TestInput>()
                              .AsParallel()
                              .WithRule("rule1")
                                .WithPredicate(async (_, _) =>
                                {
                                  //Ensure second rule gets into it's execution
                                  await Task.Delay(100);
                                  throw new EngineHaltException();
                                })
                                .WithAction((_, i) =>
                                {
                                  i.Items.Add("rule1");
                                  return Task.CompletedTask;
                                })
                              .EndRule()
                              .WithRule("rule2")
                                .WithPredicate((_, _) => Task.FromResult(1D))
                                .WithAction(async (_, i, t) =>
                                {
                                  await Task.Delay(1000, t);
                                  if (!t.IsCancellationRequested)
                                    i.Items.Add("rule2");
                                })
                              .EndRule()
                              .Build();
    var input = new TestInput();
    var stopwatch = new Stopwatch();
    stopwatch.Start();
    var task = engine.ApplyAsync(input);
    await task;
    stopwatch.Stop();
    //Engine was halted in first rule.
    //Nothing should have modified the input.
    //In addition, the delay should have been cancelled, and processing should be less than 1 second
    Assert.Empty(input.Items);
    Assert.True(stopwatch.ElapsedMilliseconds < 1000);
    Assert.False(task.IsFaulted);
  }

  [Fact]
  public async Task TestExceptionCancellationInDoesApply()
  {
    var engine = ProbabilisticEngineBuilder.ForInputAsync<TestInput>()
                              .AsParallel()
                              .WithRule("rule1")
                                .WithPredicate(async (_, _) =>
                                {
                                  //Ensure second rule gets into it's execution
                                  await Task.Delay(100);
                                  throw new();
                                })
                                .WithAction((_, i) =>
                                {
                                  i.Items.Add("rule1");
                                  return Task.CompletedTask;
                                })
                              .EndRule()
                              .WithRule("rule2")
                                .WithPredicate((_, _) => Task.FromResult(1D))
                                .WithAction(async (_, i, t) =>
                                {
                                  await Task.Delay(1000, t);
                                  if (!t.IsCancellationRequested)
                                    i.Items.Add("rule2");
                                })
                              .EndRule()
                              .Build();
    var input = new TestInput();
    var stopwatch = new Stopwatch();
    stopwatch.Start();
    //Exception should bubble up
    await Assert.ThrowsAsync<Exception>(() => engine.ApplyAsync(input));
    stopwatch.Stop();
    //Engine was halted in first rule.
    //Nothing should have modified the input.
    //In addition, the delay should have been cancelled, and processing should be less than 1 second
    Assert.Empty(input.Items);
    Assert.True(stopwatch.ElapsedMilliseconds < 1000);
  }

  [Fact]
  public async Task TestHaltCancellationInApply()
  {
    var engine = ProbabilisticEngineBuilder.ForInputAsync<TestInput>()
                              .AsParallel()
                              .WithRule("rule1")
                                .WithPredicate((_, _) => Task.FromResult(1D))
                                .WithAction(async (_, _) =>
                                {
                                  //Ensure second rule gets into its execution
                                  await Task.Delay(100);
                                  throw new EngineHaltException();
                                })
                              .EndRule()
                              .WithRule("rule2")
                                .WithPredicate((_, _) => Task.FromResult(1D))
                                .WithAction(async (_, i, t) =>
                                {
                                  await Task.Delay(1000, t);
                                  if (!t.IsCancellationRequested)
                                    i.Items.Add("rule2");
                                })
                              .EndRule()
                              .Build();
    var input = new TestInput();
    var stopwatch = new Stopwatch();
    stopwatch.Start();
    var task = engine.ApplyAsync(input);
    await task;
    stopwatch.Stop();
    //Engine was halted in first rule.
    //Nothing should have modified the input.
    //In addition, the delay should have been cancelled, and processing should be less than 1 second
    Assert.Empty(input.Items);
    Assert.True(stopwatch.ElapsedMilliseconds < 1000);
  }

  [Fact]
  public async Task TestHaltCancellationInSerial()
  {
    var engine = ProbabilisticEngineBuilder.ForInputAsync<TestInput>()
                              .AsParallel()
                              .WithRule("rule1")
                              .WithPredicate((_, _) => Task.FromResult(1D))
                              .WithAction(async (_, _) =>
                              {
                                //Ensure second rule gets into its execution
                                await Task.Delay(100);
                                throw new EngineHaltException();
                              })
                              .EndRule()
                              .WithRule("rule2")
                              .WithPredicate((_, _) => Task.FromResult(1D))
                              .WithAction(async (_, i, t) =>
                              {
                                await Task.Delay(2000, t);
                                if (!t.IsCancellationRequested)
                                  i.Items.Add("rule2");
                              })
                              .EndRule()
                              .Build();
    var input = new TestInput();
    var input2 = new TestInput();
    var stopwatch = new Stopwatch();
    stopwatch.Start();
    var task = engine.ApplyAsync(new[] { input, input2 });
    await task;
    stopwatch.Stop();
    //Engine was halted in first rule.
    //Nothing should have modified the input.
    Assert.Empty(input.Items);
  }

  [Fact]
  public async Task TestTaskCancelledExceptionSerial()
  {
    var engine = ProbabilisticEngineBuilder.ForInputAsync<TestInput>()
                              .AsParallel()
                              .WithRule("rule1")
                              .WithPredicate((_, _) => Task.FromResult(1D))
                              .WithAction(async (_, _) =>
                              {
                                //Ensure second rule gets into its execution
                                await Task.Delay(100);
                                var token = new CancellationTokenSource().Token;
                                throw new TaskCanceledException("", null, token);
                              })
                              .EndRule()
                              .WithRule("rule2")
                              .WithPredicate((_, _) => Task.FromResult(1D))
                              .WithAction(async (_, i, t) =>
                              {
                                await Task.Delay(1000, t);
                                if (!t.IsCancellationRequested)
                                  i.Items.Add("rule2");
                              })
                              .EndRule()
                              .Build();
    var input = new TestInput();
    var input2 = new TestInput();
    var stopwatch = new Stopwatch();
    stopwatch.Start();
    await Assert.ThrowsAsync<TaskCanceledException>(() => engine.ApplyAsync(new[] { input, input2 }));
  }

  [Fact]
  public async Task TestHaltCancellationThowOnCancelParallel()
  {
    var engine = ProbabilisticEngineBuilder.ForInputAsync<TestInput>()
                              .AsParallel()
                              .WithRule("rule1")
                                .WithPredicate((_, _) => Task.FromResult(1D))
                                .WithAction(async (_, _) =>
                                {
                                  //Ensure second rule gets into its execution
                                  await Task.Delay(100);
                                  throw new EngineHaltException();
                                })
                              .EndRule()
                              .WithRule("rule2")
                                .WithPredicate((_, _) => Task.FromResult(1D))
                                .WithAction(async (_, i, t) =>
                                {
                                  await Task.Delay(200, t);
                                  t.ThrowIfCancellationRequested();
                                  i.Items.Add("rule2");
                                })
                              .EndRule()
                              .Build();
    var input = new TestInput();
    var stopwatch = new Stopwatch();
    stopwatch.Start();
    var task = engine.ApplyAsync(input);
    await task;
    //We shouldn't get an exception here... we should be catching them.
    Assert.False(task.IsFaulted);
  }

  [Fact]
  public async Task TestHaltCancellationThowOnCancelSerial()
  {
    var engine = ProbabilisticEngineBuilder.ForInputAsync<TestInput>()
                              .WithRule("rule1")
                                .WithPredicate((_, _) => Task.FromResult(1D))
                                .WithAction(async (_, _) =>
                                {
                                  //Ensure second rule gets into its execution
                                  await Task.Delay(100);
                                  throw new EngineHaltException();
                                })
                              .EndRule()
                              .WithRule("rule2")
                                .WithPredicate((_, _) => Task.FromResult(1D))
                                .WithAction(async (_, i, t) =>
                                {
                                  await Task.Delay(200, t);
                                  t.ThrowIfCancellationRequested();
                                  i.Items.Add("rule2");
                                })
                              .EndRule()
                              .Build();
    var input = new TestInput();
    var stopwatch = new Stopwatch();
    stopwatch.Start();
    var task = engine.ApplyAsync(input);
    await task;
    //We shouldn't get faulted or an exception here... the user meant to do this.
    Assert.False(task.IsFaulted);
    Assert.True(task.IsCompletedSuccessfully);
  }

  [Fact]
  public async Task TestHaltCancellationInApplyParallelInputs()
  {
    var engine = ProbabilisticEngineBuilder.ForInputAsync<TestInput>()
                              .AsParallel()
                              .WithRule("rule1")
                                .WithPredicate((_, _) => Task.FromResult(1D))
                                .WithAction(async (_, _) =>
                                {
                                  //Ensure second rule gets into its execution
                                  await Task.Delay(100);
                                  throw new EngineHaltException();
                                })
                              .EndRule()
                              .WithRule("rule2")
                                .WithPredicate((_, _) => Task.FromResult(1D))
                                .WithAction(async (_, i, t) =>
                                {
                                  await Task.Delay(1000, t);
                                  if (!t.IsCancellationRequested)
                                    i.Items.Add("rule2");
                                })
                              .EndRule()
                              .Build();
    var input = new TestInput();
    var input2 = new TestInput();
    var stopwatch = new Stopwatch();
    stopwatch.Start();
    await engine.ApplyAsync(new[] { input, input2 });
    stopwatch.Stop();
    //Engine was halted in first rule.
    //Nothing should have modified the input.
    //In addition, the delay should have been cancelled, and processing should be less than 1 second
    Assert.Empty(input.Items);
    Assert.Empty(input2.Items);
    Assert.True(stopwatch.ElapsedMilliseconds < 1000);
  }

  [Fact]
  public async Task TestExceptionCancellationInApply()
  {
    var engine = ProbabilisticEngineBuilder.ForInputAsync<TestInput>()
                              .AsParallel()
                              .WithRule("rule1")
                                .WithPredicate((_, _) => Task.FromResult(1D))
                                .WithAction(async (_, _) =>
                                {
                                  //Ensure second rule gets into its execution
                                  await Task.Delay(100);
                                  throw new();
                                })
                              .EndRule()
                              .WithRule("rule2")
                                .WithPredicate((_, _) => Task.FromResult(1D))
                                .WithAction(async (_, i, t) =>
                                {
                                  await Task.Delay(1000, t);
                                  if (!t.IsCancellationRequested)
                                    i.Items.Add("rule2");
                                })
                              .EndRule()
                              .Build();
    var input = new TestInput();
    var stopwatch = new Stopwatch();
    stopwatch.Start();
    await Assert.ThrowsAsync<Exception>(() => engine.ApplyAsync(input));
    stopwatch.Stop();
    //Engine was halted in first rule.
    //Nothing should have modified the input.
    //In addition, the delay should have been cancelled, and processing should be less than 1 second
    Assert.Empty(input.Items);
    Assert.True(stopwatch.ElapsedMilliseconds < 1000);
  }

  [Fact]
  public async Task TestParallel()
  {
    var engine = ProbabilisticEngineBuilder.ForInputAsync<TestInput>()
                              .AsParallel()
                              .WithRule("rule1")
                                .WithAction(async (_, i) =>
                                {
                                  //Ensure second rule gets into its execution
                                  await Task.Delay(200);
                                  i.Items.Add("rule1");
                                })
                              .EndRule()
                              .WithRule("rule2")
                                .WithAction(async (_, i) =>
                                {
                                  await Task.Delay(100);
                                  i.Items.Add("rule2");
                                })
                              .EndRule()
                              .Build();
    var input = new TestInput();
    await engine.ApplyAsync(input);
    Assert.Equal(2, input.Items.Count);
    Assert.Equal("rule2", input.Items.First());
    Assert.Equal("rule1", input.Items.Last());
  }

  [Fact]
  public async Task TestParallelWithParallelInputs()
  {
    var engine = ProbabilisticEngineBuilder.ForInputAsync<TestInput>()
                              .AsParallel()
                              .WithRule("rule1")
                                .WithAction(async (_, i) =>
                                {
                                  //Ensure second rule gets into its execution
                                  await Task.Delay(200);
                                  i.Items.Add("rule1");
                                })
                              .EndRule()
                              .WithRule("rule2")
                                .WithAction(async (_, i) =>
                                {
                                  await Task.Delay(100);
                                  i.Items.Add("rule2");
                                })
                              .EndRule()
                              .Build();
    var input = new TestInput();
    var input2 = new TestInput();
    var stopwatch = new Stopwatch();
    stopwatch.Start();
    await engine.ApplyAsync(new[] { input, input2 }, null, true);
    stopwatch.Stop();
    Assert.Equal(2, input.Items.Count);
    Assert.Equal("rule2", input.Items.First());
    Assert.Equal("rule1", input.Items.Last());
    Assert.Equal(2, input.Items.Count);
    Assert.Equal("rule2", input.Items.First());
    Assert.Equal("rule1", input.Items.Last());
    //Since inputs and rules are parallelized, it should take less than 300 millis
    Assert.True(stopwatch.ElapsedMilliseconds < 300);
  }

  [Fact]
  public async Task TestSerialWithParallelInputs()
  {
    var engine = ProbabilisticEngineBuilder.ForInputAsync<TestInput>()
                              .WithRule("rule1")
                                .WithAction(async (_, i) =>
                                {
                                  //Ensure second rule gets into its execution
                                  await Task.Delay(200);
                                  i.Items.Add("rule1");
                                })
                              .EndRule()
                              .WithRule("rule2")
                                .WithAction(async (_, i) =>
                                {
                                  await Task.Delay(100);
                                  i.Items.Add("rule2");
                                })
                              .EndRule()
                              .Build();
    var input = new TestInput();
    var input2 = new TestInput();
    var stopwatch = new Stopwatch();
    stopwatch.Start();
    await engine.ApplyAsync(new[] { input, input2 }, null, true);
    stopwatch.Stop();
    Assert.Equal(2, input.Items.Count);
    Assert.Equal("rule1", input.Items.First());
    Assert.Equal("rule2", input.Items.Last());
    Assert.Equal(2, input.Items.Count);
    Assert.Equal("rule1", input.Items.First());
    Assert.Equal("rule2", input.Items.Last());
  }

  [Fact]
  public async Task TestSerial()
  {
    var engine = ProbabilisticEngineBuilder.ForInputAsync<TestInput>()
                              .WithRule("rule1")
                                .WithAction(async (_, i) =>
                                {
                                  //Ensure second rule gets into its execution
                                  await Task.Delay(200);
                                  i.Items.Add("rule1");
                                })
                              .EndRule()
                              .WithRule("rule2")
                                .WithAction(async (_, i) =>
                                {
                                  await Task.Delay(100);
                                  i.Items.Add("rule2");
                                })
                              .EndRule()
                              .Build();
    var input = new TestInput();
    await engine.ApplyAsync(input);
    Assert.Equal(2, input.Items.Count);
    Assert.Equal("rule1", input.Items.First());
    Assert.Equal("rule2", input.Items.Last());
  }

  [Fact]
  public async Task TestStream()
  {
    var engine = ProbabilisticEngineBuilder.ForInputAsync<TestInput>()
                              .WithRule("rule1")
                                .WithAction(async (_, i) =>
                                {
                                  //Ensure second rule gets into its execution
                                  await Task.Delay(200);
                                  i.Items.Add("rule1");
                                })
                              .EndRule()
                              .WithRule("rule2")
                                .WithAction(async (_, i) =>
                                {
                                  await Task.Delay(100);
                                  i.Items.Add("rule2");
                                })
                              .EndRule()
                              .Build();
    var input = new TestInput();
    var input2 = new TestInput();
    await engine.ApplyAsync(new[] { input, input2 }.ToAsyncEnumerable());
  }

  [Fact]
  public async Task StreamCatchEngineException()
  {
    var engine = ProbabilisticEngineBuilder.ForInputAsync<TestInput>()
                              .WithRule("rule1")
                                .WithAction(async (_, _) => throw new())
                              .EndRule()
                              .WithExceptionHandler(ExceptionHandlers.HaltEngine)
                              .Build();
    var input = new TestInput();
    var input2 = new TestInput();
    var context = new EngineContext();
    await engine.ApplyAsync(new[] { input, input2 }.ToAsyncEnumerable(), context);
    Assert.IsType<EngineHaltException>(context.GetLastException());
  }

  [Fact]
  public async Task StreamParallelCatchEngineException()
  {
    var engine = ProbabilisticEngineBuilder.ForInputAsync<TestInput>()
                              .WithRule("rule1")
                                .WithAction(async (_, _) => throw new())
                              .EndRule()
                              .WithExceptionHandler(ExceptionHandlers.HaltEngine)
                              .AsParallel()
                              .Build();
    var input = new TestInput();
    var input2 = new TestInput();
    var context = new EngineContext();
    await engine.ApplyAsync(new[] { input, input2 }.ToAsyncEnumerable(), context);
    Assert.IsType<EngineHaltException>(context.GetLastException());
  }

  [Fact]
  public async Task TestStreamParallel()
  {
    var engine = ProbabilisticEngineBuilder.ForInputAsync<TestInput>()
                              .WithRule("rule1")
                                .WithAction(async (_, i) =>
                                {
                                  await Task.Delay(200);
                                  i.Items.Add("rule1");
                                })
                              .EndRule()
                              .WithRule("rule2")
                                .WithAction(async (_, i) =>
                                {
                                  await Task.Delay(100);
                                  i.Items.Add("rule2");
                                })
                              .EndRule()
                              .AsParallel()
                              .Build();
    var input = new TestInput();
    var input2 = new TestInput();
    await engine.ApplyAsync(new[] { input, input2 }.ToAsyncEnumerable());
    Assert.Equal(2, input.Items.Count);
    Assert.Equal("rule2", input.Items.First());
    Assert.Equal("rule1", input.Items.Last());
  }

  [Fact]
  public async Task ApplyException()
  {
    var testPreRule = new TestExceptionPreRule(false);
    var engine = new RuleEngine<TestInput>(new Rule<TestInput>[] { testPreRule });
    var input = new TestInput();
    var context = new EngineContext();
    await Assert.ThrowsAsync<Exception>(() => engine.ApplyAsync(input, context));
    Assert.Null(context.GetLastException());
    Assert.True(input.InputFlag);
  }

  [Fact]
  public async Task ApplyEngineException()
  {
    var testPreRule = new LambdaRule<TestInput>("test", async (_, _, _) => 1D, async (_, _, _) => throw new EngineHaltException("Test", null));
    var engine = new RuleEngine<TestInput>(new [] { testPreRule });
    var input = new TestInput();
    var context = new EngineContext();
    await engine.ApplyAsync(input, context);
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
  public async Task ApplyItemException()
  {
    var testPreRule = new LambdaRule<TestInput>("test", async (_, _, _) => 1D, async (_, _, _) => throw new ItemHaltException());
    var engine = new RuleEngine<TestInput>(new [] { testPreRule });
    var input = new TestInput();
    var context = new EngineContext();
    await engine.ApplyAsync(input, context);
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
  public async Task ApplyExceptionHandler()
  {
    var testPreRule = new LambdaRule<TestInput>("test", async (_, _, _) => 1D, async (_, _, _) => throw new());
    var testPreRule2 = new LambdaRule<TestInput>("test2", async (_, _, _) => 1D, async (_, i, _) => i.InputFlag = true);
    var engine = new RuleEngine<TestInput>(new [] { testPreRule, testPreRule2 }, false, ExceptionHandlers.HaltEngine);
    var input = new TestInput();
    var context = new EngineContext();
    await engine.ApplyAsync(input, context);
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
  public async Task ApplyExceptionHandlerItemException()
  {
    var testPreRule = new LambdaRule<TestInput>("test", async (_, _, _) => 1D, async (_, _, _) => throw new());
    var testPreRule2 = new LambdaRule<TestInput>("test2", async (_, _, _) => 1D, async (_, i, _) => i.InputFlag = true);
    var engine = new RuleEngine<TestInput>(new [] { testPreRule, testPreRule2 }, false, ExceptionHandlers.HaltItem);
    var input = new TestInput();
    var context = new EngineContext();
    await engine.ApplyAsync(input, context);
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
  public async Task ApplyExceptionHandlerThrow()
  {
    var testPreRule = new LambdaRule<TestInput>("test", async (_, _, _) =>1D, async (_, _, _) => throw new());
    var testPreRule2 = new LambdaRule<TestInput>("test2", async (_, _, _) => 1D, async (_, i, _) => i.InputFlag = true);
    var engine = new RuleEngine<TestInput>(new [] { testPreRule, testPreRule2 }, false, ExceptionHandlers.Rethrow);
    var input = new TestInput();
    var context = new EngineContext();
    var exception = await Assert.ThrowsAsync<Exception>(() => engine.ApplyAsync(input, context));
    Assert.Null(context.GetLastException());
    Assert.IsNotType<EngineException>(exception);
    Assert.False(input.InputFlag);
  }

  [Fact]
  public async Task ApplyExceptionHandlerThrowException()
  {
    var testPreRule = new LambdaRule<TestInput>("test", async (_, _, _) => 1D, async (_, _, _) => throw new());
    var testPreRule2 = new LambdaRule<TestInput>("test2", async (_, _, _) => 1D, async (_, i, _) => i.InputFlag = true);
    var engine = new RuleEngine<TestInput>(new [] { testPreRule, testPreRule2 }, false,
        new LambdaExceptionHandler((_, _, _, _, _) => throw new InvalidOperationException()));
    var input = new TestInput();
    var context = new EngineContext();
    await Assert.ThrowsAsync<InvalidOperationException>(() => engine.ApplyAsync(input, context));
    Assert.Null(context.GetLastException());
    Assert.False(input.InputFlag);
  }

  [Fact]
  public async Task ApplyExceptionHandlerIgnore()
  {
    var testPreRule = new LambdaRule<TestInput>("test", async (_, _, _) => 1D, async (_, _, _) => throw new());
    var testPreRule2 = new LambdaRule<TestInput>("test2", async (_, _, _) => 1D, async (_, i, _) => i.InputFlag = true);
    var engine = new RuleEngine<TestInput>(new [] { testPreRule, testPreRule2 }, false, ExceptionHandlers.Ignore);
    var input = new TestInput();
    var context = new EngineContext();
    await engine.ApplyAsync(input, context);
    Assert.Null(context.GetLastException());
    Assert.True(input.InputFlag);
  }

  [Fact]
  public async Task ApplyManyHandleEngineHalt()
  {
    var testPreRule = new LambdaRule<TestInput>(
        "test",
        async (_, _, _) => 1D,
        async (_, i, _) =>
        {
          if (i.InputFlag)
            throw new EngineHaltException();
          i.InputFlag = true;
        });
    var input = new TestInput { InputFlag = true };
    var input2 = new TestInput();
    var engine = new RuleEngine<TestInput>(new IRule<TestInput>[] { testPreRule });
    var context = new EngineContext();
    await engine.ApplyAsync(new[] { input, input2 }, context);
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
  public async Task ApplyManyHandleItemHalt()
  {
    var testPreRule = new LambdaRule<TestInput>(
        "test",
        async (_, _, _) => 1D,
        async (_, i, _) =>
        {
          if (i.InputFlag)
            throw new ItemHaltException();
          i.InputFlag = true;
        });
    var input = new TestInput { InputFlag = true };
    var input2 = new TestInput();
    var engine = new RuleEngine<TestInput>(
        new IRule<TestInput>[] { testPreRule });
    var context = new EngineContext();
    await engine.ApplyAsync(new[] { input, input2 }, context);
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
  public async Task ApplyManyHandleExceptionItemHalt()
  {
    var testPreRule = new LambdaRule<TestInput>(
        "test",
        async (_, _, _) => 1D,
        async (_, i, _) =>
        {
          if (i.InputFlag)
            throw new("Test", null);
          i.InputFlag = true;
        });
    var input = new TestInput { InputFlag = true };
    var input2 = new TestInput();
    var engine = new RuleEngine<TestInput>(
        new IRule<TestInput>[] { testPreRule },
        false,
        ExceptionHandlers.HaltItem
    );
    var context = new EngineContext();
    await engine.ApplyAsync(new[] { input, input2 }, context);
    Assert.True(input.InputFlag);
    Assert.True(input2.InputFlag);
    var ex = context.GetLastException();
    Assert.NotNull(ex);
    Assert.IsType<ItemHaltException>(ex);
    Assert.Equal(testPreRule, ex.Rule);
    Assert.Equal(input, ex.Input);
    Assert.Null(ex.Output);
    Assert.NotNull(ex.Context);
  }

  [Fact]
  public async Task ApplyManyHandleExceptionEngineHalt()
  {
    var testPreRule = new LambdaRule<TestInput>(
        "test",
        async (_, _, _) => 1D,
        async (_, i, _) =>
        {
          if (i.InputFlag)
            throw new();
          i.InputFlag = true;
        });
    var input = new TestInput { InputFlag = true };
    var input2 = new TestInput();
    var engine = new RuleEngine<TestInput>(
        new [] { testPreRule },
        false,
        ExceptionHandlers.HaltEngine
    );
    var context = new EngineContext();
    await engine.ApplyAsync(new[] { input, input2 }, context);
    Assert.True(input.InputFlag);
    Assert.False(input2.InputFlag);
    Assert.NotNull(context.GetLastException());
    var exception = context.GetLastException();
    Assert.IsType<EngineHaltException>(exception);
    Assert.Equal(testPreRule, exception.Rule);
    Assert.Equal(input, exception.Input);
    Assert.Null(exception.Output);
    Assert.NotNull(exception.Context);
  }

  [Fact]
  public async Task ApplyManyHandleExceptionEngineThrow()
  {
    var testPreRule = new LambdaRule<TestInput>(
        "test",
        async (_, _, _) => 1D,
        async (_, i, _) =>
        {
          if (i.InputFlag)
            throw new();
          i.InputFlag = true;
        });
    var input = new TestInput { InputFlag = true };
    var input2 = new TestInput();
    var engine = new RuleEngine<TestInput>(
        new IRule<TestInput>[] { testPreRule },
        false,
        ExceptionHandlers.Rethrow
    );
    var context = new EngineContext();
    var exception = await Assert.ThrowsAsync<Exception>(() => engine.ApplyAsync(new[] { input, input2 }, context));
    Assert.True(input.InputFlag);
    Assert.False(input2.InputFlag);
    Assert.Null(context.GetLastException());
    Assert.IsNotType<EngineHaltException>(exception);
  }

  [Fact]
  public async Task DoesApplyException()
  {
    var testPreRule = new TestExceptionPreRule(true);
    var engine = new RuleEngine<TestInput>(new Rule<TestInput>[] { testPreRule }, false, ExceptionHandlers.Rethrow);
    var input = new TestInput();
    var context = new EngineContext();
    var exception = await Assert.ThrowsAsync<Exception>(() => engine.ApplyAsync(input, context));
    Assert.Null(context.GetLastException());
    Assert.IsNotType<EngineException>(exception);
    Assert.False(input.InputFlag);

  }


}