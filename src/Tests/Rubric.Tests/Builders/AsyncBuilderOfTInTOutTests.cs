using Rubric.Rules.Async;
using Rubric.Tests.TestRules.Async;

namespace Rubric.Tests.Builders;

public class AsyncBuilderOfTInTOutTests
{
  [Fact]
  public void EmptyBuilder()
  {
    var builder = EngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>();
    Assert.NotNull(builder.ExceptionHandler);
    Assert.NotNull(builder.Logger);
    Assert.False(builder.IsParallel);
  }

  [Fact]
  public void AsyncLambdaPreRuleConstructionThrowsOnNullOrEmpty()
  {
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithPreRule((string)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithPreRule("")
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithPreRule("foo")
                .WithAction((Func<IEngineContext, TestInput, Task>)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithPreRule("foo")
                .WithAction((Func<IEngineContext, TestInput, CancellationToken, Task>)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithPreRule("foo")
                .WithPredicate((Func<IEngineContext, TestInput, Task<bool>>)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithPreRule("foo")
                .WithPredicate((Func<IEngineContext, TestInput, CancellationToken, Task<bool>>)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithPreRule("foo")
                .ThatProvides(null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithPreRule("foo")
                .ThatProvides("")
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithPreRule("foo")
                .ThatDependsOn("")
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithPreRule("foo")
                .ThatDependsOn((string)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithPreRule("foo")
                .ThatDependsOn((Type)null)
);
  }

  [Fact]
  public void AsyncLambdaRuleConstructionThrowsOnNullOrEmpty()
  {
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithRule((string)null)
);
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>().WithRule("")
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithRule("foo")
                .WithAction((Func<IEngineContext, TestInput, TestOutput, Task>)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithRule("foo")
                .WithAction((Func<IEngineContext, TestInput, TestOutput, CancellationToken, Task>)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithRule("foo")
                .WithPredicate((Func<IEngineContext, TestInput, TestOutput, Task<bool>>)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithRule("foo")
                .WithPredicate((Func<IEngineContext, TestInput, TestOutput, CancellationToken, Task<bool>>)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>().WithRule("foo")
                .ThatProvides(null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>().WithRule("foo")
                .ThatProvides("")
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>().WithRule("foo")
                .ThatDependsOn("")
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>().WithRule("foo")
                .ThatDependsOn((string)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithRule("foo").ThatDependsOn((Type)null)
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
                              .WithPreRule(new TestDefaultPreRule())
                              .WithRule(new TestDefaultRule())
                              .WithPostRule(new TestDefaultPostRule())
                              .WithExceptionHandler(ExceptionHandlers.Ignore)
                              .AsParallel()
                              .Build();
    Assert.NotNull(engine);
    Assert.Equal(logger, engine.Logger);
    Assert.Single(engine.PreRules);
    Assert.Single(engine.Rules);
    Assert.Single(engine.PostRules);
    Assert.Equal(ExceptionHandlers.Ignore, engine.ExceptionHandler);
    Assert.True(engine.IsParallel);
  }

  [Fact]
  public async Task LambdaPostRuleConstruction()
  {
    var engine = EngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                              .WithPostRule(new TestPostRule(true))
                              .WithPostRule("test")
                              .WithPredicate((_, _) => Task.FromResult(true))
                              .WithAction((_, _) => Task.CompletedTask)
                              .ThatProvides("foo")
                              .EndRule()
                              .WithPostRule("test2")
                              .WithPredicate((_, _, _) => Task.FromResult(true))
                              .WithAction((_, _, _) => Task.CompletedTask)
                              .ThatDependsOn(typeof(TestPostRule))
                              .ThatDependsOn("test")
                              .EndRule()
                              .Build();
    Assert.Equal(3, engine.PostRules.Count());
    var rule = engine.PostRules.ElementAt(1);
    Assert.Equal("test", rule.Name);
    Assert.Contains("test", rule.Provides);
    Assert.Contains("foo", rule.Provides);
    Assert.True(await rule.DoesApply(null, null, default));
    rule = engine.PostRules.ElementAt(2);
    Assert.Contains(typeof(TestPostRule).FullName, rule.Dependencies);
    Assert.Contains("test", rule.Dependencies);
    Assert.True(await rule.DoesApply(null, null, default));
    await engine.ApplyAsync(new TestInput(), new());
  }

  [Fact]
  public void LambdaPostRuleConstructionThrowsOnNullOrEmpty()
  {
    Assert.Throws<ArgumentException>(() =>
         EngineBuilder
             .ForInputAndOutputAsync<TestInput, TestOutput>()
             .WithPostRule((string)null)
    );
    Assert.Throws<ArgumentException>(() =>
         EngineBuilder
             .ForInputAndOutputAsync<TestInput, TestOutput>().WithPostRule("")
    );
    Assert.Throws<ArgumentNullException>(
      () =>
         EngineBuilder
             .ForInputAndOutputAsync<TestInput, TestOutput>()
             .WithPostRule("foo")
             .WithAction((Func<IEngineContext, TestOutput, Task>)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
          EngineBuilder
             .ForInputAndOutputAsync<TestInput, TestOutput>()
             .WithPostRule("foo")
             .WithAction((Func<IEngineContext, TestOutput, CancellationToken, Task>)null)
    );
    Assert.Throws<ArgumentNullException>(
      () =>
         EngineBuilder
             .ForInputAndOutputAsync<TestInput, TestOutput>()
             .WithPostRule("foo")
             .WithPredicate((Func<IEngineContext, TestOutput, CancellationToken, Task<bool>>)null)
    );
    Assert.Throws<ArgumentNullException>(
      () =>
         EngineBuilder
             .ForInputAndOutputAsync<TestInput, TestOutput>()
             .WithPostRule("foo")
             .WithPredicate((Func<IEngineContext, TestOutput, Task<bool>>)null)
    );
    Assert.Throws<ArgumentException>(
      () =>
        EngineBuilder
            .ForInputAndOutputAsync<TestInput, TestOutput>()
            .WithPostRule("foo").ThatProvides(null)
    );
    Assert.Throws<ArgumentException>(
      () =>
        EngineBuilder
            .ForInputAndOutputAsync<TestInput, TestOutput>()
            .WithPostRule("foo").ThatProvides("")
    );
    Assert.Throws<ArgumentException>(
      () =>
        EngineBuilder
            .ForInputAndOutputAsync<TestInput, TestOutput>()
            .WithPostRule("foo").ThatDependsOn("")
    );
    Assert.Throws<ArgumentException>(
      () =>
        EngineBuilder
            .ForInputAndOutputAsync<TestInput, TestOutput>()
            .WithPostRule("foo").ThatDependsOn((string)null)
    );
    Assert.Throws<ArgumentNullException>(
      () =>
        EngineBuilder
            .ForInputAndOutputAsync<TestInput, TestOutput>()
            .WithPostRule("foo").ThatDependsOn((Type)null)
    );
  }

  [Fact]
  public async Task LambdaPreRuleConstruction()
  {
    var engine = EngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                              .WithPreRule(new TestPreRule(true))
                              .WithPreRule("test")
                              .WithPredicate((_, _) => Task.FromResult(true))
                              .WithAction((_, _) => Task.CompletedTask)
                              .ThatProvides("foo")
                              .EndRule()
                              .WithPreRule("test2")
                              .WithPredicate((_, _, _) => Task.FromResult(true))
                              .WithAction((_, _, _) => Task.CompletedTask)
                              .ThatDependsOn(typeof(TestPreRule))
                              .ThatDependsOn("test")
                              .EndRule()
                              .Build();
    Assert.Equal(3, engine.PreRules.Count());
    var rule = engine.PreRules.ElementAt(1);
    Assert.Equal("test", rule.Name);
    Assert.Contains("foo", rule.Provides);
    Assert.Contains("test", rule.Provides);
    Assert.True(await rule.DoesApply(null, null, default));
    rule = engine.PreRules.ElementAt(2);
    Assert.Contains("test", rule.Dependencies);
    Assert.Contains(typeof(TestPreRule).FullName, rule.Dependencies);
    Assert.True(await rule.DoesApply(null, null, default));
    await engine.ApplyAsync(new TestInput(), new());
  }

  [Fact]
  public async Task LambdaRuleConstruction()
  {
    var engine = EngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                              .WithRule(new TestRule(true))
                              .WithRule("test")
                              .WithPredicate((_, _, _) => Task.FromResult(true))
                              .WithAction((_, _, _) => Task.CompletedTask)
                              .ThatProvides("test1")
                              .EndRule()
                              .WithRule("test2")
                              .WithPredicate((_, _, _, _) => Task.FromResult(true))
                              .WithAction((_, _, _, _) => Task.CompletedTask)
                              .ThatDependsOn("test1")
                              .ThatDependsOn(typeof(TestRule))
                              .EndRule()
                              .Build();
    Assert.Equal(3, engine.Rules.Count());
    var rule = engine.Rules.ElementAt(1);
    Assert.Equal("test", rule.Name);
    Assert.Contains("test1", rule.Provides);
    Assert.True(await rule.DoesApply(null, null, null, default));
    rule = engine.Rules.ElementAt(2);
    Assert.Contains(typeof(TestRule).FullName, rule.Dependencies);
    Assert.Contains("test1", rule.Dependencies);
    Assert.True(await rule.DoesApply(null, null, null, default));
    await engine.ApplyAsync(new TestInput(), new());
  }


  [Fact]
  public async Task RuleWrapping()
  {
    var engine = EngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                              .WithPreRule(new TestRules.TestPreRule(true))
                              .WithRule(new TestRules.TestRule(true))
                              .WithPostRule(new TestRules.TestPostRule(true))
                              .Build();
    Assert.Single(engine.PreRules);
    var preRule = engine.PreRules.ElementAt(0);
    Assert.Equal($"{typeof(TestRules.TestPreRule)} (wrapped async)", preRule.Name);
    Assert.Single(engine.Rules);
    var rule = engine.Rules.ElementAt(0);
    Assert.Equal($"{typeof(TestRules.TestRule)} (wrapped async)", rule.Name);
    Assert.Single(engine.PostRules);
    var postRule = engine.PostRules.ElementAt(0);
    Assert.Equal($"{typeof(TestRules.TestPostRule)} (wrapped async)", postRule.Name);
    Assert.True(await preRule.DoesApply(null, null, default));
    Assert.True(await rule.DoesApply(null, null, null, default));
    Assert.True(await postRule.DoesApply(null, null, default));
  }

  [Fact]
  public async Task RuleWrappingMultiple()
  {
    var engine = EngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                              .WithPreRules(new [] {new TestRules.TestPreRule(true)})
                              .WithRules(new [] {new TestRules.TestRule(true)})
                              .WithPostRules(new [] {new TestRules.TestPostRule(true)})
                              .Build();
    Assert.Single(engine.PreRules);
    var preRule = engine.PreRules.ElementAt(0);
    Assert.Equal($"{typeof(TestRules.TestPreRule)} (wrapped async)", preRule.Name);
    Assert.Single(engine.Rules);
    var rule = engine.Rules.ElementAt(0);
    Assert.Equal($"{typeof(TestRules.TestRule)} (wrapped async)", rule.Name);
    Assert.Single(engine.PostRules);
    var postRule = engine.PostRules.ElementAt(0);
    Assert.Equal($"{typeof(TestRules.TestPostRule)} (wrapped async)", postRule.Name);
    Assert.True(await preRule.DoesApply(null, null, default));
    Assert.True(await rule.DoesApply(null, null, null, default));
    Assert.True(await postRule.DoesApply(null, null, default));
  }

  [Fact]
  public void TypeAttributeDependency()
  {
    var engine = EngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                              .WithPreRules(new IRule<TestInput>[] { new DepTestAttrPreRule(true), new DepTestAttrPreRule2(true)})
                              .WithRules(new IRule<TestInput, TestOutput>[] { new DepTestAttrRule(true), new DepTestAttrRule2(true) })
                              .WithPostRules(new IRule<TestOutput>[] { new DepTestAttrPostRule(true), new DepTestAttrPostRule2(true)})
                              .Build();
    Assert.NotNull(engine);
  }

  [Fact]
  public void TypeAttributeDependencyWrapped()
  {
    var engine = EngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                              .WithPreRules(new [] { new TestRules.DepTestAttrPreRule(true), new TestRules.DepTestAttrPreRule2(true) })
                              .WithRules(new [] { new TestRules.DepTestAttrRule(true), new TestRules.DepTestAttrRule2(true) })
                              .WithPostRules(new [] { new TestRules.DepTestAttrPostRule(true), new TestRules.DepTestAttrPostRule2(true) })
                              .Build();
    Assert.NotNull(engine);
  }
}
