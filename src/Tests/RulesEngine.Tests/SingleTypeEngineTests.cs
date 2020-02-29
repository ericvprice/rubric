using System.Collections.Generic;
using Microsoft.Extensions.Logging.Abstractions;
using RulesEngine.Rules;
using RulesEngine.Tests.TestRules;
using Xunit;

namespace RulesEngine.Tests
{
    public class SingleTypeEngineTests
    {
        [Fact]
        public void Applies()
        {
            var rule = new TestDefaultPreRule();
            var input = new TestInput();
            var engine = new RulesEngine<TestInput>(
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
            var engine = new RulesEngine<TestInput>(
                new IRule<TestInput>[] { rule }
            );
            engine.Apply(new[] { input, input2 });
            Assert.True(input.InputFlag);
            Assert.True(input2.InputFlag);
        }

        [Fact]
        public void AppliesOrder()
        {
            var rule = new TestDefaultPreRule();
            var rule2 = new TestPreRule(true, false);
            var input = new TestInput();
            var engine = new RulesEngine<TestInput>(
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
        public void Constructor()
        {
            var logger = new TestLogger();
            var ruleSet = new Ruleset<TestInput>();
            var engine = new RulesEngine<TestInput>(ruleSet, logger);
            Assert.Equal(logger, engine.Logger);
            Assert.Empty(engine.Rules);
        }

        [Fact]
        public void ConstructorNullRules()
        {
            var engine = new RulesEngine<TestInput>((IEnumerable<IRule<TestInput>>)null, null);
            Assert.Empty(engine.Rules);
        }

        [Fact]
        public void ConstructorNullLogger()
        {
            var ruleSet = new Ruleset<TestInput>();
            var engine = new RulesEngine<TestInput>(ruleSet);
            Assert.Equal(NullLogger.Instance, engine.Logger);
        }


        [Fact]
        public void ApplyException()
        {
            var testPreRule = new TestExceptionPreRule(false);
            var engine = new RulesEngine<TestInput>(new Rule<TestInput>[] { testPreRule });
            var input = new TestInput();
            var exception = Assert.Throws<EngineHaltException>(() => engine.Apply(input));
            Assert.Equal(testPreRule, exception.Rule);
            Assert.Equal(input, exception.Input);
            Assert.Null(exception.Output);
            Assert.NotNull(exception.Context);
            Assert.True(input.InputFlag);
        }

        [Fact]
        public void DoesApplyException()
        {
            var testPreRule = new TestExceptionPreRule(true);
            var engine = new RulesEngine<TestInput>(new Rule<TestInput>[] { testPreRule });
            var input = new TestInput();
            var exception = Assert.Throws<EngineHaltException>(() => engine.Apply(input));
            Assert.Equal(testPreRule, exception.Rule);
            Assert.Equal(input, exception.Input);
            Assert.Null(exception.Output);
            Assert.NotNull(exception.Context);
            Assert.False(input.InputFlag);
        }

        [Fact]
        public void NotApplies()
        {
            var rule = new TestPreRule(false);
            var input = new TestInput();
            var engine = new RulesEngine<TestInput>(
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
            var engine = new RulesEngine<TestInput>(ruleSet, logger);
            Assert.False(engine.IsAsync);
            Assert.False(engine.IsParallel);
            Assert.Equal(typeof(TestInput), engine.InputType);
            Assert.Equal(typeof(TestInput), engine.OutputType);
        }

    }
}