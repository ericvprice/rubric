using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using RulesEngine.Builder;
using RulesEngine.Rules;
using RulesEngine.Rules.Async;
using RulesEngine.Tests.TestRules;
using RulesEngine.Tests.TestRules.Async;
using Xunit;

namespace RulesEngine.Tests
{
    public class AsyncEngineOfTTests
    {
        [Fact]
        public async Task AppliesOrder()
        {
            var rule = new TestDefaultAsyncPreRule();
            var rule2 = new TestAsyncPreRule(true, false);
            var input = new TestInput();
            var engine = new AsyncRulesEngine<TestInput>(
                new IAsyncRule<TestInput>[] { rule, rule2 }
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
                new IAsyncRule<TestInput>[] { rule2, rule }
            );
            await engine.ApplyAsync(input);
            Assert.True(input.InputFlag);
        }

        [Fact]
        public void Constructor()
        {
            var logger = new TestLogger();
            var ruleSet = new AsyncRuleset<TestInput>();
            var engine = new AsyncRulesEngine<TestInput>(ruleSet, false, logger);
            Assert.Equal(logger, engine.Logger);
            Assert.False(engine.IsParallel);
        }

        [Fact]
        public void Properties()
        {
            var logger = new TestLogger();
            var ruleSet = new AsyncRuleset<TestInput>();
            var engine = new AsyncRulesEngine<TestInput>(ruleSet, false, logger);
            Assert.True(engine.IsAsync);
            Assert.False(engine.IsParallel);
            Assert.Equal(typeof(TestInput), engine.InputType);
            Assert.Equal(typeof(TestInput), engine.OutputType);
        }

        [Fact]
        public void PropertiesParallel()
        {
            var logger = new TestLogger();
            var ruleSet = new AsyncRuleset<TestInput>();
            var engine = new AsyncRulesEngine<TestInput>(ruleSet, true, logger);
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
            var engine = new AsyncRulesEngine<TestInput>(ruleSet, false, logger);
            Assert.Equal(logger, engine.Logger);
        }

        [Fact]
        public void ConstructorWithSyncRuleset()
        {
            var logger = new TestLogger();
            var ruleSet = new Ruleset<TestInput>();
            ruleSet.AddRule(new TestPreRule(true));
            var engine = new AsyncRulesEngine<TestInput>(ruleSet, false, logger);
            Assert.NotEmpty(engine.Rules);
        }


        [Fact]
        public async Task Applies()
        {
            var rule = new TestDefaultAsyncPreRule();
            var input = new TestInput();
            var engine = new AsyncRulesEngine<TestInput>(
                new IAsyncRule<TestInput>[] { rule }
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
                new IAsyncRule<TestInput>[] { rule }
            );
            await engine.ApplyAsync(new[] { input, input2 });
            Assert.True(input.InputFlag);
            Assert.True(input2.InputFlag);
        }

        [Fact]
        public async Task AppliesManyEmpty()
        {
            var rule = new TestDefaultAsyncPreRule();
            var engine = new AsyncRulesEngine<TestInput>(
                new IAsyncRule<TestInput>[] { rule }
            );
            await engine.ApplyAsync(new TestInput[0]);
            //Shouldn't throw
        }

        [Fact]
        public async Task NotApplies()
        {
            var rule = new TestAsyncPreRule(false);
            var input = new TestInput();
            var engine = new AsyncRulesEngine<TestInput>(
                new IAsyncRule<TestInput>[] { rule }
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
            var exception = await Assert.ThrowsAsync<EngineExecutionException>(() => engine.ApplyAsync(input, output));
            Assert.Equal(testPreRule, exception.Rule);
            Assert.Equal(input, exception.Input);
            Assert.Null(exception.Output);
            Assert.NotNull(exception.Context);
            Assert.True(input.InputFlag);
        }

        [Fact]
        public async Task DoesApplyAsyncException()
        {
            var testPreRule = new TestExceptionAsyncPreRule(false);
            var engine =
                new AsyncRulesEngine<TestInput>(
                    new AsyncRule<TestInput>[] { testPreRule });
            var input = new TestInput();
            var exception = await Assert.ThrowsAsync<EngineExecutionException>(async () => await engine.ApplyAsync(input));
            Assert.Equal(testPreRule, exception.Rule);
            Assert.Equal(input, exception.Input);
            Assert.Null(exception.Output);
            Assert.NotNull(exception.Context);
            Assert.True(input.InputFlag);
        }

        [Fact]
        public async Task TestEngineHalt()
        {
            var engine =
                new AsyncRulesEngine<TestInput>(
                    new AsyncRule<TestInput>[] { new AsyncPreHaltRule(), new TestAsyncPreRule(true) });
            var input = new TestInput();
            await engine.ApplyAsync(input);
            //Engine should have halted on first rule, second rule not run
            Assert.False(input.InputFlag);
        }

        [Fact]
        public async Task TestHaltInDoesApply()
        {
            var engine = EngineBuilder.ForInputAsync<TestInput>()
                                      .WithRule("rule1")
                                        .WithPredicate((c, i) => throw new EngineHaltException())
                                        .WithAction((c, i) => { i.Items.Add("rule1"); return Task.CompletedTask; })
                                      .EndRule()
                                      .WithRule("rule2")
                                        .WithPredicate((c, i) => Task.FromResult(true))
                                        .WithAction((c, i) => { i.Items.Add("rule2"); return Task.CompletedTask; })
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
            var engine = EngineBuilder.ForInputAsync<TestInput>()
                                      .WithRule("rule1")
                                        .WithPredicate((c, i) => Task.FromResult(true))
                                        .WithAction((c, i) => throw new EngineHaltException())
                                      .EndRule()
                                      .WithRule("rule2")
                                        .WithPredicate((c, i) => Task.FromResult(true))
                                        .WithAction((c, i) => { i.Items.Add("rule2"); return Task.CompletedTask; })
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
            var engine = EngineBuilder.ForInputAsync<TestInput>()
                                      .AsParallel()
                                      .WithRule("rule1")
                                        .WithPredicate(async (c, i) =>
                                        {
                                            //Ensure second rule gets into it's execution
                                            await Task.Delay(100);
                                            throw new EngineHaltException();
                                        })
                                        .WithAction((c, i) =>
                                        {
                                            i.Items.Add("rule1");
                                            return Task.CompletedTask;
                                        })
                                      .EndRule()
                                      .WithRule("rule2")
                                        .WithPredicate((c, i) => Task.FromResult(true))
                                        .WithAction(async (c, i, t) =>
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
            var engine = EngineBuilder.ForInputAsync<TestInput>()
                                      .AsParallel()
                                      .WithRule("rule1")
                                        .WithPredicate(async (c, i) =>
                                        {
                                            //Ensure second rule gets into it's execution
                                            await Task.Delay(100);
                                            throw new Exception();
                                        })
                                        .WithAction((c, i) =>
                                        {
                                            i.Items.Add("rule1");
                                            return Task.CompletedTask;
                                        })
                                      .EndRule()
                                      .WithRule("rule2")
                                        .WithPredicate((c, i) => Task.FromResult(true))
                                        .WithAction(async (c, i, t) =>
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
            await Assert.ThrowsAsync<EngineExecutionException>(() => engine.ApplyAsync(input));
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
            var engine = EngineBuilder.ForInputAsync<TestInput>()
                                      .AsParallel()
                                      .WithRule("rule1")
                                        .WithPredicate((c, i) => Task.FromResult(true))
                                        .WithAction(async (c, i) =>
                                        {
                                            //Ensure second rule gets into its execution
                                            await Task.Delay(100);
                                            throw new EngineHaltException();
                                        })
                                      .EndRule()
                                      .WithRule("rule2")
                                        .WithPredicate((c, i) => Task.FromResult(true))
                                        .WithAction(async (c, i, t) =>
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
            var engine = EngineBuilder.ForInputAsync<TestInput>()
                                      .AsParallel()
                                      .WithRule("rule1")
                                      .WithPredicate((c, i) => Task.FromResult(true))
                                      .WithAction(async (c, i) =>
                                      {
                                          //Ensure second rule gets into its execution
                                          await Task.Delay(100);
                                          throw new EngineHaltException();
                                      })
                                      .EndRule()
                                      .WithRule("rule2")
                                      .WithPredicate((c, i) => Task.FromResult(true))
                                      .WithAction(async (c, i, t) =>
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
            var task = engine.ApplyAsync(new[] { input, input2 });
            await task;
            stopwatch.Stop();
            //Engine was halted in first rule.
            //Nothing should have modified the input.
            //In addition, the delay should have been cancelled, and processing should be less than 1 second
            Assert.Empty(input.Items);
            Assert.True(stopwatch.ElapsedMilliseconds < 1000);
        }

        [Fact]
        public async Task TestTaskCacncelledExceptionSerial()
        {
            var engine = EngineBuilder.ForInputAsync<TestInput>()
                                      .AsParallel()
                                      .WithRule("rule1")
                                      .WithPredicate((c, i) => Task.FromResult(true))
                                      .WithAction(async (c, i) =>
                                      {
                                          //Ensure second rule gets into its execution
                                          await Task.Delay(100);
                                          throw new TaskCanceledException();
                                      })
                                      .EndRule()
                                      .WithRule("rule2")
                                      .WithPredicate((c, i) => Task.FromResult(true))
                                      .WithAction(async (c, i, t) =>
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
            await Assert.ThrowsAsync<EngineExecutionException>(() => engine.ApplyAsync(new[] { input, input2 }));
        }

        [Fact]
        public async Task TestHaltCancellationThowOnCancelParallel()
        {
            var engine = EngineBuilder.ForInputAsync<TestInput>()
                                      .AsParallel()
                                      .WithRule("rule1")
                                        .WithPredicate((c, i) => Task.FromResult(true))
                                        .WithAction(async (c, i) =>
                                        {
                                            //Ensure second rule gets into its execution
                                            await Task.Delay(100);
                                            throw new EngineHaltException();
                                        })
                                      .EndRule()
                                      .WithRule("rule2")
                                        .WithPredicate((c, i) => Task.FromResult(true))
                                        .WithAction(async (c, i, t) =>
                                        {
                                            await Task.Delay(200);
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
            var engine = EngineBuilder.ForInputAsync<TestInput>()
                                      .WithRule("rule1")
                                        .WithPredicate((c, i) => Task.FromResult(true))
                                        .WithAction(async (c, i) =>
                                        {
                                            //Ensure second rule gets into its execution
                                            await Task.Delay(100);
                                            throw new EngineHaltException();
                                        })
                                      .EndRule()
                                      .WithRule("rule2")
                                        .WithPredicate((c, i) => Task.FromResult(true))
                                        .WithAction(async (c, i, t) =>
                                        {
                                            await Task.Delay(200);
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
            var engine = EngineBuilder.ForInputAsync<TestInput>()
                                      .AsParallel()
                                      .WithRule("rule1")
                                        .WithPredicate((c, i) => Task.FromResult(true))
                                        .WithAction(async (c, i) =>
                                        {
                                            //Ensure second rule gets into its execution
                                            await Task.Delay(100);
                                            throw new EngineHaltException();
                                        })
                                      .EndRule()
                                      .WithRule("rule2")
                                        .WithPredicate((c, i) => Task.FromResult(true))
                                        .WithAction(async (c, i, t) =>
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
            var engine = EngineBuilder.ForInputAsync<TestInput>()
                                      .AsParallel()
                                      .WithRule("rule1")
                                        .WithPredicate((c, i) => Task.FromResult(true))
                                        .WithAction(async (c, i) =>
                                        {
                                            //Ensure second rule gets into its execution
                                            await Task.Delay(100);
                                            throw new Exception();
                                        })
                                      .EndRule()
                                      .WithRule("rule2")
                                        .WithPredicate((c, i) => Task.FromResult(true))
                                        .WithAction(async (c, i, t) =>
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
            await Assert.ThrowsAsync<EngineExecutionException>(() => engine.ApplyAsync(input));
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
            var engine = EngineBuilder.ForInputAsync<TestInput>()
                                      .AsParallel()
                                      .WithRule("rule1")
                                        .WithAction(async (c, i) =>
                                        {
                                            //Ensure second rule gets into its execution
                                            await Task.Delay(200);
                                            i.Items.Add("rule1");
                                        })
                                      .EndRule()
                                      .WithRule("rule2")
                                        .WithAction(async (c, i) =>
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
            var engine = EngineBuilder.ForInputAsync<TestInput>()
                                      .AsParallel()
                                      .WithRule("rule1")
                                        .WithAction(async (c, i) =>
                                        {
                                            //Ensure second rule gets into its execution
                                            await Task.Delay(200);
                                            i.Items.Add("rule1");
                                        })
                                      .EndRule()
                                      .WithRule("rule2")
                                        .WithAction(async (c, i) =>
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
            await engine.ApplyAsync(new[] { input, input2 }, true);
            stopwatch.Stop();
            Assert.Equal(2, input.Items.Count);
            Assert.Equal("rule2", input.Items.First());
            Assert.Equal("rule1", input.Items.Last());
            Assert.Equal(2, input.Items.Count);
            Assert.Equal("rule2", input.Items.First());
            Assert.Equal("rule1", input.Items.Last());
            //It should take ~200 millis to run on a single rule since rules are parallelized.
            //Since inputs are parallelized, it should take less than 300 millis
            Assert.True(stopwatch.ElapsedMilliseconds < 300);
            Assert.True(stopwatch.ElapsedMilliseconds >= 200);
        }

        [Fact]
        public async Task TestSerialWithParallelInputs()
        {
            var engine = EngineBuilder.ForInputAsync<TestInput>()
                                      .WithRule("rule1")
                                        .WithAction(async (c, i) =>
                                        {
                                            //Ensure second rule gets into its execution
                                            await Task.Delay(200);
                                            i.Items.Add("rule1");
                                        })
                                      .EndRule()
                                      .WithRule("rule2")
                                        .WithAction(async (c, i) =>
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
            await engine.ApplyAsync(new[] { input, input2 }, true);
            stopwatch.Stop();
            Assert.Equal(2, input.Items.Count);
            Assert.Equal("rule1", input.Items.First());
            Assert.Equal("rule2", input.Items.Last());
            Assert.Equal(2, input.Items.Count);
            Assert.Equal("rule1", input.Items.First());
            Assert.Equal("rule2", input.Items.Last());
            //It should take >300 millis to run on a single rule since rules are not parallelized.
            //Since inputs are parallelized, it should take less than 400
            Assert.True(stopwatch.ElapsedMilliseconds >= 300);
            Assert.True(stopwatch.ElapsedMilliseconds < 400);
        }

        [Fact]
        public async Task TestSerial()
        {
            var engine = EngineBuilder.ForInputAsync<TestInput>()
                                      .WithRule("rule1")
                                        .WithAction(async (c, i) =>
                                        {
                                            //Ensure second rule gets into its execution
                                            await Task.Delay(200);
                                            i.Items.Add("rule1");
                                        })
                                      .EndRule()
                                      .WithRule("rule2")
                                        .WithAction(async (c, i) =>
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
    }
}