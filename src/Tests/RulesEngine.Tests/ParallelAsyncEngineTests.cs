using RulesEngine.Rules.Async;
using Xunit;
using RulesEngine.Tests.TestRules.Async;
using RulesEngine.Builder;
using System.Linq;
using System.Threading.Tasks;

namespace RulesEngine.Tests
{

    public class ParallelAsyncEngineTests
    {

        [Fact]
        public async Task PreApplies()
        {
            var rule = new TestDefaultAsyncPreRule();
            var input = new TestInput();
            var output = new TestOutput();
            var engine = new AsyncRulesEngine<TestInput, TestOutput>(
                new IAsyncPreRule<TestInput>[] { rule },
                null,
                null
            )
            {
                ProcessInParallel = true
            };
            await engine.ApplyAsync(input, output);
            Assert.True(input.InputFlag);
        }

        [Fact]
        public async Task PostApplies()
        {
            var rule = new TestDefaultAsyncPostRule();
            var input = new TestInput();
            var output = new TestOutput();
            var engine = new AsyncRulesEngine<TestInput, TestOutput>(
                null,
                null,
                new IAsyncPostRule<TestOutput>[] { rule }
            )
            {
                ProcessInParallel = true
            };
            await engine.ApplyAsync(input, output);
            Assert.True(output.TestFlag);
        }

        [Fact]
        public async Task Applies()
        {
            var rule = new TestDefaultAsyncRule();
            var input = new TestInput();
            var output = new TestOutput();
            var engine = new AsyncRulesEngine<TestInput, TestOutput>(
                null,
                new IAsyncRule<TestInput, TestOutput>[] { rule },
                null
            )
            {
                ProcessInParallel = true
            };
            await engine.ApplyAsync(input, output);
            Assert.True(input.InputFlag);
            Assert.True(output.TestFlag);
        }

        [Fact]
        public async Task PreNotApplies()
        {
            var rule = new TestAsyncPreRule(false);
            var input = new TestInput();
            var output = new TestOutput();
            var engine = new AsyncRulesEngine<TestInput, TestOutput>(
                new IAsyncPreRule<TestInput>[] { rule },
                null,
                null
            )
            {
                ProcessInParallel = true
            };
            await engine.ApplyAsync(input, output);
            Assert.False(input.InputFlag);
        }

        [Fact]
        public async Task PostNotApplies()
        {
            var rule = new TestAsyncPostRule(false);
            var input = new TestInput();
            var output = new TestOutput();
            var engine = new AsyncRulesEngine<TestInput, TestOutput>(
                null,
                null,
                new IAsyncPostRule<TestOutput>[] { rule }
            )
            {
                ProcessInParallel = true
            };
            await engine.ApplyAsync(input, output);
            Assert.False(output.TestFlag);
        }

        [Fact]
        public async Task NotApplies()
        {
            var rule = new TestAsyncRule(false);
            var input = new TestInput();
            var output = new TestOutput();
            var engine = new AsyncRulesEngine<TestInput, TestOutput>(
                null,
                new IAsyncRule<TestInput, TestOutput>[] { rule },
                null
            )
            {
                ProcessInParallel = true
            };
            await engine.ApplyAsync(input, output);
            Assert.False(input.InputFlag);
            Assert.False(output.TestFlag);
        }


        [Fact]
        public async Task WrapPreDoesApplyAsyncException()
        {
            var engine = new AsyncRulesEngine<TestInput, TestOutput>(
                new AsyncPreRule<TestInput>[] { new TestExceptionAsyncPreRule(true) },
                null,
                null)
            {
                ProcessInParallel = true
            };
            var input = new TestInput();
            var output = new TestOutput();
            await Assert.ThrowsAsync<EngineHaltException>(async () => await engine.ApplyAsync(input, output));
            Assert.False(input.InputFlag);
        }

        [Fact]
        public async Task WrapPreApplyAsyncException()
        {
            var engine = new AsyncRulesEngine<TestInput, TestOutput>(
                new AsyncPreRule<TestInput>[] { new TestExceptionAsyncPreRule(false) },
                null,
                null)
            {
                ProcessInParallel = true
            };
            var input = new TestInput();
            var output = new TestOutput();
            await Assert.ThrowsAsync<EngineHaltException>(() => engine.ApplyAsync(input, output));
            Assert.True(input.InputFlag);
        }

        [Fact]
        public async Task WrapPostDoesApplyAsyncException()
        {
            var engine = new AsyncRulesEngine<TestInput, TestOutput>(
                null,
                null,
                new AsyncPostRule<TestOutput>[] { new TestExceptionAsyncPostRule(true) })
            {
                ProcessInParallel = true
            };
            var input = new TestInput();
            var output = new TestOutput();
            await Assert.ThrowsAsync<EngineHaltException>(() => engine.ApplyAsync(input, output));
            Assert.False(output.TestFlag);
        }

        [Fact]
        public async Task WrapPostApplyAsyncException()
        {
            var engine = new AsyncRulesEngine<TestInput, TestOutput>(
                null,
                null,
                new AsyncPostRule<TestOutput>[] { new TestExceptionAsyncPostRule(false) })
            {
                ProcessInParallel = true
            };
            var input = new TestInput();
            var output = new TestOutput();
            await Assert.ThrowsAsync<EngineHaltException>(() => engine.ApplyAsync(input, output));
            Assert.True(output.TestFlag);
        }

        [Fact]
        public async Task WrapDoesApplyAsyncException()
        {
            var engine = new AsyncRulesEngine<TestInput, TestOutput>(
                null,
                new AsyncRule<TestInput, TestOutput>[] { new TestExceptionAsyncRule(true) },
                null)
            {
                ProcessInParallel = true
            };
            var input = new TestInput();
            var output = new TestOutput();
            await Assert.ThrowsAsync<EngineHaltException>(() => engine.ApplyAsync(input, output));
            Assert.False(input.InputFlag);
            Assert.False(output.TestFlag);
        }

        [Fact]
        public async Task WrapApplyAsyncException()
        {
            var engine = new AsyncRulesEngine<TestInput, TestOutput>(
                null,
                new AsyncRule<TestInput, TestOutput>[] { new TestExceptionAsyncRule(false) },
                null)
            {
                ProcessInParallel = true
            };
            var input = new TestInput();
            var output = new TestOutput();
            await Assert.ThrowsAsync<EngineHaltException>(() => engine.ApplyAsync(input, output));
            Assert.True(input.InputFlag);
            Assert.True(output.TestFlag);
        }

        [Fact]
        public async Task FullRun()
        {
            var engine = EngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                                      .WithPreRule("test")
                                          .WithPredicate((c, i) => Task.FromResult(true))
                                          .WithAction((c, i) => { i.Items.Add("pre"); return Task.CompletedTask; })
                                          .EndRule()
                                      .WithRule("test")
                                           .WithPredicate((c, i, o) => Task.FromResult(true))
                                           .WithAction((c, i, o) => { i.Items.Add("rule"); o.Outputs.Add("rule"); return Task.CompletedTask; })
                                           .EndRule()
                                      .WithPostRule("test")
                                           .WithPredicate((c, o) => Task.FromResult(true))
                                           .WithAction((c, o) => { o.Outputs.Add("postrule"); return Task.CompletedTask; })
                                           .EndRule()
                                       .Build();
            var input1 = new TestInput();
            var input2 = new TestInput();
            var output = new TestOutput();
            engine.ProcessInParallel = true;
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
    }
}