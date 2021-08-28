//using RulesEngine.Builder;
//using RulesEngine.Rules;
//using RulesEngine.Rules.Async;
//using RulesEngine.Tests.TestRules;
//using RulesEngine.Tests.TestRules.Async;
//using Xunit;

//namespace RulesEngine.Tests
//{
//    public class AsyncEngineTests
//    {
//        [Fact]
//        public async Task Applies()
//        {
//            var rule = new TestDefaultAsyncRule();
//            var input = new TestInput();
//            var output = new TestOutput();
//            var engine = new AsyncRulesEngine<TestInput, TestOutput>(
//                null,
//                new IAsyncRule<TestInput, TestOutput>[] { rule },
//                null
//            );
//            await engine.ApplyAsync(input, output);
//            Assert.True(input.InputFlag);
//            Assert.True(output.TestFlag);
//        }

//        [Fact]
//        public async Task AppliesOrder()
//        {
//            var rule = new TestDefaultAsyncPreRule();
//            var rule2 = new TestAsyncPreRule(true, false);
//            var input = new TestInput();
//            var output = new TestOutput();
//            var engine = new AsyncRulesEngine<TestInput, TestOutput>(
//                new IAsyncRule<TestInput>[] { rule, rule2 },
//                null,
//                null
//            );
//            await engine.ApplyAsync(input, output);
//            Assert.False(input.InputFlag);
//        }

//        [Fact]
//        public async Task AppliesOrderReverse()
//        {
//            var rule = new TestDefaultAsyncPreRule();
//            var rule2 = new TestAsyncPreRule(true, false);
//            var input = new TestInput { InputFlag = true };
//            var output = new TestOutput();
//            var engine = new AsyncRulesEngine<TestInput, TestOutput>(
//                new IAsyncRule<TestInput>[] { rule2, rule },
//                null,
//                null
//            );
//            await engine.ApplyAsync(input, output);
//            Assert.True(input.InputFlag);
//        }

//        [Fact]
//        public void Constructor()
//        {
//            var logger = new TestLogger();
//            var ruleSet = new AsyncRuleset<TestInput, TestOutput>();
//            var engine = new AsyncRulesEngine<TestInput, TestOutput>(ruleSet, false, null, logger);
//            Assert.Equal(logger, engine.Logger);
//            Assert.False(engine.IsParallel);
//        }

//        [Fact]
//        public void ConstructorNullLogger()
//        {
//            var ruleSet = new AsyncRuleset<TestInput, TestOutput>();
//            var engine = new AsyncRulesEngine<TestInput, TestOutput>(ruleSet);
//            Assert.NotNull(engine.Logger);
//        }

//        [Fact]
//        public void ConstructorParallel()
//        {
//            var ruleSet = new AsyncRuleset<TestInput, TestOutput>();
//            var engine = new AsyncRulesEngine<TestInput, TestOutput>(ruleSet, true);
//            Assert.True(engine.IsParallel);
//        }

//        [Fact]
//        public void ConstructorWithEmptySyncRuleset()
//        {
//            var logger = new TestLogger();
//            var ruleSet = new Ruleset<TestInput, TestOutput>();
//            var engine = new AsyncRulesEngine<TestInput, TestOutput>(ruleSet, false, null, logger);
//            Assert.Equal(logger, engine.Logger);
//        }

//        [Fact]
//        public void ConstructorWithSyncRuleset()
//        {
//            var logger = new TestLogger();
//            var ruleSet = new Ruleset<TestInput, TestOutput>();
//            ruleSet.AddPreRule(new TestPreRule(true));
//            ruleSet.AddPostRule(new TestPostRule(true));
//            ruleSet.AddRule(new TestRule(true));
//            var engine = new AsyncRulesEngine<TestInput, TestOutput>(ruleSet, false, null, logger);
//            Assert.NotEmpty(engine.PreRules);
//            Assert.NotEmpty(engine.Rules);
//            Assert.NotEmpty(engine.PostRules);
//        }

//        [Fact]
//        public async Task FullRun()
//        {
//            var engine = EngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
//                                      .WithPreRule("test")
//                                      .WithPredicate((c, i) => Task.FromResult(true))
//                                      .WithAction((c, i) =>
//                                      {
//                                          i.Items.Add("pre");
//                                          return Task.CompletedTask;
//                                      })
//                                      .EndRule()
//                                      .WithRule("test")
//                                      .WithPredicate((c, i, o) => Task.FromResult(true))
//                                      .WithAction((c, i, o) =>
//                                      {
//                                          i.Items.Add("rule");
//                                          o.Outputs.Add("rule");
//                                          return Task.CompletedTask;
//                                      })
//                                      .EndRule()
//                                      .WithPostRule("test")
//                                      .WithPredicate((c, o) => Task.FromResult(true))
//                                      .WithAction((c, o) =>
//                                      {
//                                          o.Outputs.Add("postrule");
//                                          return Task.CompletedTask;
//                                      })
//                                      .EndRule()
//                                      .Build();
//            var input1 = new TestInput();
//            var input2 = new TestInput();
//            var output = new TestOutput();
//            await engine.ApplyAsync(new[] { input1, input2 }, output);
//            Assert.Equal(2, input1.Items.Count);
//            Assert.Contains("pre", input1.Items);
//            Assert.Contains("rule", input1.Items);
//            Assert.Equal(2, input2.Items.Count);
//            Assert.Contains("pre", input2.Items);
//            Assert.Contains("rule", input2.Items);
//            Assert.Equal(3, output.Outputs.Count);
//            Assert.Equal(2, output.Outputs.Count(o => o == "rule"));
//            Assert.Single(output.Outputs.Where(o => o == "postrule"));
//        }

//        [Fact]
//        public async Task NotApplies()
//        {
//            var rule = new TestAsyncRule(false);
//            var input = new TestInput();
//            var output = new TestOutput();
//            var engine = new AsyncRulesEngine<TestInput, TestOutput>(
//                null,
//                new IAsyncRule<TestInput, TestOutput>[] { rule },
//                null
//            );
//            await engine.ApplyAsync(input, output);
//            Assert.False(input.InputFlag);
//            Assert.False(output.TestFlag);
//        }

//        [Fact]
//        public async Task PostApplies()
//        {
//            var rule = new TestDefaultAsyncPostRule();
//            var input = new TestInput();
//            var output = new TestOutput();
//            var engine = new AsyncRulesEngine<TestInput, TestOutput>(
//                null,
//                null,
//                new IAsyncRule<TestOutput>[] { rule }
//            );
//            await engine.ApplyAsync(input, output);
//            Assert.True(output.TestFlag);
//        }

//        [Fact]
//        public async Task PostNotApplies()
//        {
//            var rule = new TestAsyncPostRule(false);
//            var input = new TestInput();
//            var output = new TestOutput();
//            var engine = new AsyncRulesEngine<TestInput, TestOutput>(
//                null,
//                null,
//                new IAsyncRule<TestOutput>[] { rule }
//            );
//            await engine.ApplyAsync(input, output);
//            Assert.False(output.TestFlag);
//        }

//        [Fact]
//        public async Task PreApplies()
//        {
//            var rule = new TestDefaultAsyncPreRule();
//            var input = new TestInput();
//            var output = new TestOutput();
//            var engine = new AsyncRulesEngine<TestInput, TestOutput>(
//                new IAsyncRule<TestInput>[] { rule },
//                null,
//                null
//            );
//            await engine.ApplyAsync(input, output);
//            Assert.True(input.InputFlag);
//        }

//        [Fact]
//        public async Task PreNotApplies()
//        {
//            var rule = new TestAsyncPreRule(false);
//            var input = new TestInput();
//            var output = new TestOutput();
//            var engine = new AsyncRulesEngine<TestInput, TestOutput>(
//                new IAsyncRule<TestInput>[] { rule },
//                null,
//                null
//            );
//            await engine.ApplyAsync(input, output);
//            Assert.False(input.InputFlag);
//        }

//        [Fact]
//        public async Task ApplyAsyncException()
//        {
//            var engine = new AsyncRulesEngine<TestInput, TestOutput>(
//                null, new AsyncRule<TestInput, TestOutput>[] { new TestExceptionAsyncRule(false) }, null);
//            var input = new TestInput();
//            var output = new TestOutput();
//            await Assert.ThrowsAsync<EngineExecutionException>(() => engine.ApplyAsync(input, output));
//            Assert.True(input.InputFlag);
//            Assert.True(output.TestFlag);
//        }

//        [Fact]
//        public async Task DoesApplyAsyncException()
//        {
//            var engine = new AsyncRulesEngine<TestInput, TestOutput>(
//                null, new AsyncRule<TestInput, TestOutput>[] { new TestExceptionAsyncRule(true) }, null);
//            var input = new TestInput();
//            var output = new TestOutput();
//            await Assert.ThrowsAsync<EngineExecutionException>(() => engine.ApplyAsync(input, output));
//            Assert.False(input.InputFlag);
//            Assert.False(output.TestFlag);
//        }

//        [Fact]
//        public async Task PostApplyAsyncException()
//        {
//            var testPostRule = new TestExceptionAsyncPostRule(false);
//            var engine =
//                new AsyncRulesEngine<TestInput, TestOutput>(
//                    null, null, new AsyncRule<TestOutput>[] { testPostRule });
//            var input = new TestInput();
//            var output = new TestOutput();
//            var exception = await Assert.ThrowsAsync<EngineExecutionException>(() => engine.ApplyAsync(input, output));
//            Assert.True(output.TestFlag);
//            Assert.Equal(testPostRule, exception.Rule);
//            Assert.Null(exception.Input);
//            Assert.Equal(output, exception.Output);
//            Assert.NotNull(exception.Context);
//        }

//        [Fact]
//        public async Task PostDoesApplyAsyncException()
//        {
//            var testPostRule = new TestExceptionAsyncPostRule(false);
//            var engine =
//                new AsyncRulesEngine<TestInput, TestOutput>(
//                    null, null, new AsyncRule<TestOutput>[] { testPostRule });
//            var input = new TestInput();
//            var output = new TestOutput();
//            var exception = await Assert.ThrowsAsync<EngineExecutionException>(() => engine.ApplyAsync(input, output));
//            Assert.Equal(testPostRule, exception.Rule);
//            Assert.Null(exception.Input);
//            Assert.Equal(output, exception.Output);
//            Assert.NotNull(exception.Context);
//            Assert.True(output.TestFlag);
//        }

//        [Fact]
//        public async Task PreApplyAsyncException()
//        {
//            var testPreRule = new TestExceptionAsyncPreRule(false);
//            var engine =
//                new AsyncRulesEngine<TestInput, TestOutput>(
//                    new AsyncRule<TestInput>[] { testPreRule }, null, null);
//            var input = new TestInput();
//            var output = new TestOutput();
//            var exception = await Assert.ThrowsAsync<EngineExecutionException>(() => engine.ApplyAsync(input, output));
//            Assert.Equal(testPreRule, exception.Rule);
//            Assert.Equal(input, exception.Input);
//            Assert.Null(exception.Output);
//            Assert.NotNull(exception.Context);
//            Assert.True(input.InputFlag);
//        }

//        [Fact]
//        public async Task PreDoesApplyAsyncException()
//        {
//            var testPreRule = new TestExceptionAsyncPreRule(false);
//            var engine =
//                new AsyncRulesEngine<TestInput, TestOutput>(
//                    new AsyncRule<TestInput>[] { testPreRule }, null, null);
//            var input = new TestInput();
//            var output = new TestOutput();
//            var exception = await Assert.ThrowsAsync<EngineExecutionException>(async () => await engine.ApplyAsync(input, output));
//            Assert.Equal(testPreRule, exception.Rule);
//            Assert.Equal(input, exception.Input);
//            Assert.Null(exception.Output);
//            Assert.NotNull(exception.Context);
//            Assert.True(input.InputFlag);
//        }


//    }
//}