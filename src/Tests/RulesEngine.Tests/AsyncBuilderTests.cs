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
    public class AsyncBuilderTests
    {
        [Fact]
        public void AsyncLambdaPreRuleConstructionThrowsOnNullOrEmpty()
        {
            Assert.Throws<ArgumentException>(() =>
                                                 EngineBuilder
                                                     .ForInputAndOutputAsync<TestInput, TestOutput>()
                                                     .WithPreRule((string) null)
            );
            Assert.Throws<ArgumentException>(() =>
                                                 EngineBuilder
                                                     .ForInputAndOutputAsync<TestInput, TestOutput>().WithPreRule("")
            );
            Assert.Throws<ArgumentNullException>(() =>
                                                     EngineBuilder
                                                         .ForInputAndOutputAsync<TestInput, TestOutput>()
                                                         .WithPreRule("foo").WithAction(null)
            );
            Assert.Throws<ArgumentNullException>(() =>
                                                     EngineBuilder
                                                         .ForInputAndOutputAsync<TestInput, TestOutput>()
                                                         .WithPreRule("foo").WithPredicate(null)
            );
            Assert.Throws<ArgumentException>(() =>
                                                 EngineBuilder
                                                     .ForInputAndOutputAsync<TestInput, TestOutput>().WithPreRule("foo")
                                                     .ThatProvides(null)
            );
            Assert.Throws<ArgumentException>(() =>
                                                 EngineBuilder
                                                     .ForInputAndOutputAsync<TestInput, TestOutput>().WithPreRule("foo")
                                                     .ThatProvides("")
            );
            Assert.Throws<ArgumentException>(() =>
                                                 EngineBuilder
                                                     .ForInputAndOutputAsync<TestInput, TestOutput>().WithPreRule("foo")
                                                     .ThatDependsOn("")
            );
            Assert.Throws<ArgumentException>(() =>
                                                 EngineBuilder
                                                     .ForInputAndOutputAsync<TestInput, TestOutput>().WithPreRule("foo")
                                                     .ThatDependsOn((string) null)
            );
            Assert.Throws<ArgumentNullException>(() =>
                                                     EngineBuilder
                                                         .ForInputAndOutputAsync<TestInput, TestOutput>()
                                                         .WithPreRule("foo").ThatDependsOn((Type) null)
            );
        }

        [Fact]
        public void AsyncLambdaRuleConstructionThrowsOnNullOrEmpty()
        {
            Assert.Throws<ArgumentException>(() =>
                                                 EngineBuilder
                                                     .ForInputAndOutputAsync<TestInput, TestOutput>()
                                                     .WithRule((string) null)
            );
            Assert.Throws<ArgumentException>(() =>
                                                 EngineBuilder
                                                     .ForInputAndOutputAsync<TestInput, TestOutput>().WithRule("")
            );
            Assert.Throws<ArgumentNullException>(() =>
                                                     EngineBuilder
                                                         .ForInputAndOutputAsync<TestInput, TestOutput>()
                                                         .WithRule("foo").WithAction(null)
            );
            Assert.Throws<ArgumentNullException>(() =>
                                                     EngineBuilder
                                                         .ForInputAndOutputAsync<TestInput, TestOutput>()
                                                         .WithRule("foo").WithPredicate(null)
            );
            Assert.Throws<ArgumentException>(() =>
                                                 EngineBuilder
                                                     .ForInputAndOutputAsync<TestInput, TestOutput>().WithRule("foo")
                                                     .ThatProvides(null)
            );
            Assert.Throws<ArgumentException>(() =>
                                                 EngineBuilder
                                                     .ForInputAndOutputAsync<TestInput, TestOutput>().WithRule("foo")
                                                     .ThatProvides("")
            );
            Assert.Throws<ArgumentException>(() =>
                                                 EngineBuilder
                                                     .ForInputAndOutputAsync<TestInput, TestOutput>().WithRule("foo")
                                                     .ThatDependsOn("")
            );
            Assert.Throws<ArgumentException>(() =>
                                                 EngineBuilder
                                                     .ForInputAndOutputAsync<TestInput, TestOutput>().WithRule("foo")
                                                     .ThatDependsOn((string) null)
            );
            Assert.Throws<ArgumentNullException>(() =>
                                                     EngineBuilder
                                                         .ForInputAndOutputAsync<TestInput, TestOutput>()
                                                         .WithRule("foo").ThatDependsOn((Type) null)
            );
        }

        [Fact]
        public async Task EmptyEngineConstruction()
        {
            var engine = EngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                                      .Build();
            Assert.NotNull(engine);
            var input = new TestInput();
            var output = new TestOutput();
            //Just assert nothing goes wrong with a blank engine
            await engine.ApplyAsync(input, output);
        }

        [Fact]
        public async Task EmptyEngineConstructionWithLogger()
        {
            var logger = new TestLogger();
            var engine = EngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>(logger)
                                      .Build();
            Assert.NotNull(engine);
            Assert.Equal(logger, engine.Logger);
            var input = new TestInput();
            var output = new TestOutput();
            //Just assert nothing goes wrong with a blank engine
            await engine.ApplyAsync(input, output);
        }

        [Fact]
        public void EngineConstruction()
        {
            var logger = new TestLogger();
            var engine = EngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>(logger)
                                      .WithPreRule(new TestDefaultAsyncPreRule())
                                      .WithRule(new TestDefaultAsyncRule())
                                      .WithPostRule(new TestDefaultAsyncPostRule())
                                      .Build();
            Assert.NotNull(engine);
            Assert.Equal(logger, engine.Logger);
            Assert.Single(engine.PreRules);
            Assert.Single(engine.Rules);
            Assert.Single(engine.PostRules);
        }

        [Fact]
        public async Task LambdaPostRuleConstruction()
        {
            var engine = EngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                                      .WithPostRule(new TestAsyncPostRule(true))
                                      .WithPostRule("test")
                                      .WithPredicate((c, o) => Task.FromResult(true))
                                      .WithAction((c, o) => Task.CompletedTask)
                                      .ThatProvides("foo")
                                      .EndRule()
                                      .WithPostRule("test2")
                                      .WithPredicate((c, i) => Task.FromResult(true))
                                      .WithAction((c, i) => Task.CompletedTask)
                                      .ThatDependsOn(typeof(TestAsyncPostRule))
                                      .ThatDependsOn("test")
                                      .EndRule()
                                      .Build();
            Assert.Equal(3, engine.PostRules.Count());
            var rule = engine.PostRules.ElementAt(1);
            Assert.Equal("test", rule.Name);
            Assert.Contains("test", rule.Provides);
            Assert.Contains("foo", rule.Provides);
            rule = engine.PostRules.ElementAt(2);
            Assert.Contains(typeof(TestAsyncPostRule).FullName, rule.Dependencies);
            Assert.Contains("test", rule.Dependencies);
            Assert.True(await rule.DoesApply(null, null));
        }

        [Fact]
        public void LambdaPostRuleConstructionThrowsOnNullOrEmpty()
        {
            Assert.Throws<ArgumentException>(() =>
                                                 EngineBuilder
                                                     .ForInputAndOutputAsync<TestInput, TestOutput>()
                                                     .WithPostRule((string) null)
            );
            Assert.Throws<ArgumentException>(() =>
                                                 EngineBuilder
                                                     .ForInputAndOutputAsync<TestInput, TestOutput>().WithPostRule("")
            );
            Assert.Throws<ArgumentNullException>(() =>
                                                     EngineBuilder
                                                         .ForInputAndOutputAsync<TestInput, TestOutput>()
                                                         .WithPostRule("foo").WithAction(null)
            );
            Assert.Throws<ArgumentNullException>(() =>
                                                     EngineBuilder
                                                         .ForInputAndOutputAsync<TestInput, TestOutput>()
                                                         .WithPostRule("foo").WithPredicate(null)
            );
            Assert.Throws<ArgumentException>(() =>
                                                 EngineBuilder
                                                     .ForInputAndOutputAsync<TestInput, TestOutput>()
                                                     .WithPostRule("foo").ThatProvides(null)
            );
            Assert.Throws<ArgumentException>(() =>
                                                 EngineBuilder
                                                     .ForInputAndOutputAsync<TestInput, TestOutput>()
                                                     .WithPostRule("foo").ThatProvides("")
            );
            Assert.Throws<ArgumentException>(() =>
                                                 EngineBuilder
                                                     .ForInputAndOutputAsync<TestInput, TestOutput>()
                                                     .WithPostRule("foo").ThatDependsOn("")
            );
            Assert.Throws<ArgumentException>(() =>
                                                 EngineBuilder
                                                     .ForInputAndOutputAsync<TestInput, TestOutput>()
                                                     .WithPostRule("foo").ThatDependsOn((string) null)
            );
            Assert.Throws<ArgumentNullException>(() =>
                                                     EngineBuilder
                                                         .ForInputAndOutputAsync<TestInput, TestOutput>()
                                                         .WithPostRule("foo").ThatDependsOn((Type) null)
            );
        }

        [Fact]
        public async Task LambdaPreRuleConstruction()
        {
            var engine = EngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                                      .WithPreRule(new TestAsyncPreRule(true))
                                      .WithPreRule("test")
                                      .WithPredicate((c, i) => Task.FromResult(true))
                                      .WithAction((c, i) => Task.CompletedTask)
                                      .ThatProvides("foo")
                                      .EndRule()
                                      .WithPreRule("test2")
                                      .WithPredicate((c, i) => Task.FromResult(true))
                                      .WithAction((c, i) => Task.CompletedTask)
                                      .ThatDependsOn(typeof(TestAsyncPreRule))
                                      .ThatDependsOn("test")
                                      .EndRule()
                                      .Build();
            Assert.Equal(3, engine.PreRules.Count());
            var rule = engine.PreRules.ElementAt(1);
            Assert.Equal("test", rule.Name);
            Assert.Contains("foo", rule.Provides);
            Assert.Contains("test", rule.Provides);
            rule = engine.PreRules.ElementAt(2);
            Assert.Contains("test", rule.Dependencies);
            Assert.Contains(typeof(TestAsyncPreRule).FullName, rule.Dependencies);
            Assert.True(await rule.DoesApply(null, null));
        }

        [Fact]
        public async Task LambdaRuleConstruction()
        {
            var engine = EngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                                      .WithRule(new TestAsyncRule(true))
                                      .WithRule("test")
                                      .WithPredicate((c, i, o) => Task.FromResult(true))
                                      .WithAction((c, i, o) => Task.CompletedTask)
                                      .ThatProvides("test1")
                                      .EndRule()
                                      .WithRule("test2")
                                      .WithPredicate((c, i, o) => Task.FromResult(true))
                                      .WithAction((c, i, o) => Task.CompletedTask)
                                      .ThatDependsOn("test1")
                                      .ThatDependsOn(typeof(TestAsyncRule))
                                      .EndRule()
                                      .Build();
            Assert.Equal(3, engine.Rules.Count());
            var rule = engine.Rules.ElementAt(1);
            Assert.Equal("test", rule.Name);
            Assert.Contains("test1", rule.Provides);
            rule = engine.Rules.ElementAt(2);
            Assert.Contains(typeof(TestAsyncRule).FullName, rule.Dependencies);
            Assert.Contains("test1", rule.Dependencies);
            Assert.True(await rule.DoesApply(null, null, null));
        }


        [Fact]
        public async Task RuleWrapping()
        {
            var engine = EngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                                      .WithPreRule(new TestPreRule(true))
                                      .WithRule(new TestRule(true))
                                      .WithPostRule(new TestPostRule(true))
                                      .Build();
            Assert.Single(engine.PreRules);
            var preRule = engine.PreRules.ElementAt(0);
            Assert.Equal($"{typeof(TestPreRule)} (wrapped async)", preRule.Name);
            Assert.Single(engine.Rules);
            var rule = engine.Rules.ElementAt(0);
            Assert.Equal($"{typeof(TestRule)} (wrapped async)", rule.Name);
            Assert.Single(engine.PostRules);
            var postRule = engine.PostRules.ElementAt(0);
            Assert.Equal($"{typeof(TestPostRule)} (wrapped async)", postRule.Name);
            Assert.True(await preRule.DoesApply(null, null));
            Assert.True(await rule.DoesApply(null, null, null));
            Assert.True(await postRule.DoesApply(null, null));
        }

        [Fact]
        public void TypeAttributeDependency()
        {
            var engine = EngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                                      .WithPreRule(new DepTestAsyncPreRule(true))
                                      .WithPreRule(new DepTestAsyncPreRule2(true))
                                      .WithRule(new DepTestAsyncRule(true))
                                      .WithRule(new DepTestAsyncRule2(true))
                                      .WithPostRule(new DepTestAsyncPostRule(true))
                                      .WithPostRule(new DepTestAsyncPostRule2(true))
                                      .Build();
            Assert.NotNull(engine);
        }
    }
}