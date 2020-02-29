using System;
using System.Linq;
using System.Threading.Tasks;
using RulesEngine.Builder;
using RulesEngine.Tests.DependencyRules.TypeAttribute;
using RulesEngine.Tests.TestRules;
using RulesEngine.Tests.TestRules.Async;
using Xunit;

namespace RulesEngine.Tests
{
    public class SingleTypeAsyncBuilderTests
    {
        [Fact]
        public void AsyncLambdaRuleConstructionThrowsOnNullOrEmpty()
        {
            Assert.Throws<ArgumentException>(() =>
                                                 EngineBuilder
                                                     .ForInputAsync<TestInput>()
                                                     .WithRule((string)null)
            );
            Assert.Throws<ArgumentException>(() =>
                                                 EngineBuilder
                                                     .ForInputAsync<TestInput>().WithRule("")
            );
            Assert.Throws<ArgumentNullException>(() =>
                                                     EngineBuilder
                                                         .ForInputAsync<TestInput>()
                                                         .WithRule("foo").WithAction(null)
            );
            Assert.Throws<ArgumentNullException>(() =>
                                                     EngineBuilder
                                                         .ForInputAsync<TestInput>()
                                                         .WithRule("foo").WithPredicate(null)
            );
            Assert.Throws<ArgumentException>(() =>
                                                 EngineBuilder
                                                     .ForInputAsync<TestInput>().WithRule("foo")
                                                     .ThatProvides(null)
            );
            Assert.Throws<ArgumentException>(() =>
                                                 EngineBuilder
                                                     .ForInputAsync<TestInput>().WithRule("foo")
                                                     .ThatProvides("")
            );
            Assert.Throws<ArgumentException>(() =>
                                                 EngineBuilder
                                                     .ForInputAsync<TestInput>().WithRule("foo")
                                                     .ThatDependsOn("")
            );
            Assert.Throws<ArgumentException>(() =>
                                                 EngineBuilder
                                                     .ForInputAsync<TestInput>().WithRule("foo")
                                                     .ThatDependsOn((string)null)
            );
            Assert.Throws<ArgumentNullException>(() =>
                                                     EngineBuilder
                                                         .ForInputAsync<TestInput>()
                                                         .WithRule("foo").ThatDependsOn((Type)null)
            );
        }

        [Fact]
        public async Task EmptyEngineConstruction()
        {
            var engine = EngineBuilder.ForInputAsync<TestInput>()
                                      .Build();
            Assert.NotNull(engine);
            var input = new TestInput();
            //Just assert nothing goes wrong with a blank engine
            await engine.ApplyAsync(input);
        }

        [Fact]
        public void AsParallel()
        {
            var engine = EngineBuilder.ForInputAsync<TestInput>()
                                      .AsParallel()
                                      .Build();
            Assert.NotNull(engine);
            var input = new TestInput();
            Assert.True(engine.IsParallel);
        }

        [Fact]
        public async Task EmptyEngineConstructionWithLogger()
        {
            var logger = new TestLogger();
            var engine = EngineBuilder.ForInputAsync<TestInput>(logger)
                                      .Build();
            Assert.NotNull(engine);
            Assert.Equal(logger, engine.Logger);
            var input = new TestInput();
            var output = new TestOutput();
            //Just assert nothing goes wrong with a blank engine
            await engine.ApplyAsync(input);
        }

        [Fact]
        public void EngineConstruction()
        {
            var logger = new TestLogger();
            var engine = EngineBuilder.ForInputAsync<TestInput>(logger)
                                      .WithRule(new TestDefaultAsyncPreRule())
                                      .Build();
            Assert.NotNull(engine);
            Assert.Equal(logger, engine.Logger);
            Assert.Single(engine.Rules);
        }



        [Fact]
        public async Task LambdaRuleConstruction()
        {
            var engine = EngineBuilder.ForInputAsync<TestInput>()
                                      .WithRule(new TestAsyncPreRule(true))
                                      .WithRule("test")
                                      .WithPredicate((c, i) => Task.FromResult(true))
                                      .WithAction((c, i) => Task.CompletedTask)
                                      .ThatProvides("foo")
                                      .EndRule()
                                      .WithRule("test2")
                                      .WithPredicate((c, i) => Task.FromResult(true))
                                      .WithAction((c, i) => Task.CompletedTask)
                                      .ThatDependsOn(typeof(TestAsyncPreRule))
                                      .ThatDependsOn("test")
                                      .EndRule()
                                      .Build();
            Assert.Equal(3, engine.Rules.Count());
            var rule = engine.Rules.ElementAt(1);
            Assert.Equal("test", rule.Name);
            Assert.Contains("foo", rule.Provides);
            Assert.Contains("test", rule.Provides);
            rule = engine.Rules.ElementAt(2);
            Assert.Contains("test", rule.Dependencies);
            Assert.Contains(typeof(TestAsyncPreRule).FullName, rule.Dependencies);
            Assert.True(await rule.DoesApply(null, null));
        }

        [Fact]
        public async Task RuleWrapping()
        {
            var engine = EngineBuilder.ForInputAsync<TestInput>()
                                      .WithRule(new TestPreRule(true))
                                      .Build();
            Assert.Single(engine.Rules);
            var rule = engine.Rules.ElementAt(0);
            Assert.Equal($"{typeof(TestPreRule)} (wrapped async)", rule.Name);
            Assert.True(await rule.DoesApply(null, null));

        }

        [Fact]
        public void TypeAttributeDependency()
        {
            var engine = EngineBuilder.ForInputAsync<TestInput>()
                                      .WithRule(new DepTestAsyncPreRule(true))
                                      .WithRule(new DepTestAsyncPreRule2(true))
                                      .Build();
            Assert.NotNull(engine);
        }
    }
}