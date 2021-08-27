using RulesEngine.Builder;
using RulesEngine.Rules;
using RulesEngine.Tests.TestRules;
using Xunit;

namespace RulesEngine.Tests
{
    public class EngineTests
    {
        [Fact]
        public void Applies()
        {
            var rule = new TestDefaultRule();
            var input = new TestInput();
            var output = new TestOutput();
            var engine = new RulesEngine<TestInput, TestOutput>(
                null,
                new IRule<TestInput, TestOutput>[] { rule },
                null
            );
            engine.Apply(input, output);
            Assert.True(input.InputFlag);
            Assert.True(output.TestFlag);
        }

        [Fact]
        public void AppliesOrder()
        {
            var rule = new TestDefaultPreRule();
            var rule2 = new TestPreRule(true, false);
            var input = new TestInput();
            var output = new TestOutput();
            var engine = new RulesEngine<TestInput, TestOutput>(
                new IRule<TestInput>[] { rule, rule2 },
                null,
                null
            );
            engine.Apply(input, output);
            Assert.False(input.InputFlag);
        }

        [Fact]
        public void AppliesOrderReverse()
        {
            var rule = new TestDefaultPreRule();
            var rule2 = new TestPreRule(true, false);
            var input = new TestInput { InputFlag = true };
            var output = new TestOutput();
            var engine = new RulesEngine<TestInput, TestOutput>(
                new IRule<TestInput>[] { rule2, rule },
                null,
                null
            );
            engine.Apply(input, output);
            Assert.True(input.InputFlag);
        }

        [Fact]
        public void ApplyException()
        {
            var rule = new TestExceptionRule(false);
            var engine = new RulesEngine<TestInput, TestOutput>(null, new Rule<TestInput, TestOutput>[] { rule }, null);
            var input = new TestInput();
            var output = new TestOutput();
            var exception = Assert.Throws<EngineExecutionException>(() => engine.Apply(input, output));
            Assert.Equal(rule, exception.Rule);
            Assert.Equal(input, exception.Input);
            Assert.Equal(output, exception.Output);
            Assert.NotNull(exception.Context);
            Assert.True(input.InputFlag);
            Assert.True(output.TestFlag);
        }

        [Fact]
        public void Constructor()
        {
            var logger = new TestLogger();
            var ruleSet = new Ruleset<TestInput, TestOutput>();
            var engine = new RulesEngine<TestInput, TestOutput>(ruleSet, ExceptionHandlers.HaltEngine, logger);
            Assert.Equal(logger, engine.Logger);
            Assert.Equal(ExceptionHandlers.HaltEngine, engine.ExceptionHandler);
        }

        [Fact]
        public void ConstructorNullLoggerAndHandler()
        {
            var ruleSet = new Ruleset<TestInput, TestOutput>();
            var engine = new RulesEngine<TestInput, TestOutput>(ruleSet);
            Assert.NotNull(engine.Logger);
            Assert.NotNull(engine.ExceptionHandler);
        }

        [Fact]
        public void DoesApplyException()
        {
            var rule = new TestExceptionRule(true);
            var engine = new RulesEngine<TestInput, TestOutput>(null, new Rule<TestInput, TestOutput>[] { rule }, null);
            var input = new TestInput();
            var output = new TestOutput();
            var exception = Assert.Throws<EngineExecutionException>(() => engine.Apply(input, output));
            Assert.Equal(rule, exception.Rule);
            Assert.Equal(input, exception.Input);
            Assert.Equal(output, exception.Output);
            Assert.NotNull(exception.Context);
            Assert.False(input.InputFlag);
            Assert.False(output.TestFlag);
        }

        [Fact]
        public void FullRun()
        {
            var engine = EngineBuilder.ForInputAndOutput<TestInput, TestOutput>()
                                      .WithPreRule("test")
                                      .WithPredicate((c, i) => true)
                                      .WithAction((c, i) => i.Items.Add("pre"))
                                      .EndRule()
                                      .WithRule("test")
                                      .WithPredicate((c, i, o) => true)
                                      .WithAction((c, i, o) =>
                                      {
                                          i.Items.Add("rule");
                                          o.Outputs.Add("rule");
                                      })
                                      .EndRule()
                                      .WithPostRule("test")
                                      .WithPredicate((c, o) => true)
                                      .WithAction((c, o) => o.Outputs.Add("postrule"))
                                      .EndRule()
                                      .Build();
            var input1 = new TestInput();
            var input2 = new TestInput();
            var output = new TestOutput();
            engine.Apply(new[] { input1, input2 }, output);
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
        public void NotApplies()
        {
            var rule = new TestRule(false);
            var input = new TestInput();
            var output = new TestOutput();
            var engine = new RulesEngine<TestInput, TestOutput>(
                null,
                new IRule<TestInput, TestOutput>[] { rule },
                null
            );
            engine.Apply(input, output);
            Assert.False(input.InputFlag);
            Assert.False(output.TestFlag);
        }

        [Fact]
        public void PostApplies()
        {
            var rule = new TestDefaultPostRule();
            var input = new TestInput();
            var output = new TestOutput();
            var engine = new RulesEngine<TestInput, TestOutput>(
                null,
                null,
                new IRule<TestOutput>[] { rule }
            );
            engine.Apply(input, output);
            Assert.True(output.TestFlag);
        }

        [Fact]
        public void PostApplyException()
        {
            var testPostRule = new TestExceptionPostRule(false);
            var engine =
                new RulesEngine<TestInput, TestOutput>(null, null, new Rule<TestOutput>[] { testPostRule });
            var input = new TestInput();
            var output = new TestOutput();
            var exception = Assert.Throws<EngineExecutionException>(() => engine.Apply(input, output));
            Assert.Equal(testPostRule, exception.Rule);
            Assert.Null(exception.Input);
            Assert.Equal(output, exception.Output);
            Assert.NotNull(exception.Context);
            Assert.True(output.TestFlag);
        }

        [Fact]
        public void PostDoesApplyException()
        {
            var testPostRule = new TestExceptionPostRule(true);
            var engine =
                new RulesEngine<TestInput, TestOutput>(null, null, new Rule<TestOutput>[] { testPostRule });
            var input = new TestInput();
            var output = new TestOutput();
            var exception = Assert.Throws<EngineExecutionException>(() => engine.Apply(input, output));
            Assert.Equal(testPostRule, exception.Rule);
            Assert.Null(exception.Input);
            Assert.Equal(output, exception.Output);
            Assert.NotNull(exception.Context);
            Assert.False(output.TestFlag);
        }

        [Fact]
        public void PostNotApplies()
        {
            var rule = new TestPostRule(false);
            var input = new TestInput();
            var output = new TestOutput();
            var engine = new RulesEngine<TestInput, TestOutput>(
                null,
                null,
                new IRule<TestOutput>[] { rule }
            );
            engine.Apply(input, output);
            Assert.False(output.TestFlag);
        }

        [Fact]
        public void PreApplies()
        {
            var rule = new TestDefaultPreRule();
            var input = new TestInput();
            var output = new TestOutput();
            var engine = new RulesEngine<TestInput, TestOutput>(
                new IRule<TestInput>[] { rule },
                null,
                null
            );
            engine.Apply(input, output);
            Assert.True(input.InputFlag);
        }

        [Fact]
        public void PreApplyException()
        {
            var testPreRule = new TestExceptionPreRule(false);
            var engine = new RulesEngine<TestInput, TestOutput>(new Rule<TestInput>[] { testPreRule }, null, null);
            var input = new TestInput();
            var output = new TestOutput();
            var exception = Assert.Throws<EngineExecutionException>(() => engine.Apply(input, output));
            Assert.Equal(testPreRule, exception.Rule);
            Assert.Equal(input, exception.Input);
            Assert.Null(exception.Output);
            Assert.NotNull(exception.Context);
            Assert.True(input.InputFlag);
        }

        [Fact]
        public void PreDoesApplyException()
        {
            var testPreRule = new TestExceptionPreRule(true);
            var engine = new RulesEngine<TestInput, TestOutput>(new Rule<TestInput>[] { testPreRule }, null, null);
            var input = new TestInput();
            var output = new TestOutput();
            var exception = Assert.Throws<EngineExecutionException>(() => engine.Apply(input, output));
            Assert.Equal(testPreRule, exception.Rule);
            Assert.Equal(input, exception.Input);
            Assert.Null(exception.Output);
            Assert.NotNull(exception.Context);
            Assert.False(input.InputFlag);
        }

        [Fact]
        public void PreNotApplies()
        {
            var rule = new TestPreRule(false);
            var input = new TestInput();
            var output = new TestOutput();
            var engine = new RulesEngine<TestInput, TestOutput>(
                new IRule<TestInput>[] { rule },
                null,
                null
            );
            engine.Apply(input, output);
            Assert.False(input.InputFlag);
        }
    }
}