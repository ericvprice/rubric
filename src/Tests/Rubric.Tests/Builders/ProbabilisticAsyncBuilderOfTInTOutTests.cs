using Rubric.Tests.TestRules.Probabilistic.Async;

namespace Rubric.Tests.Builders;

public class ProbabilisticAsyncBuilderOfTInTOutTests
{
  [Fact]
  public void EmptyBuilder()
  {
    var builder = ProbabilisticEngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>();
    Assert.NotNull(builder.ExceptionHandler);
    Assert.NotNull(builder.Logger);
    Assert.False(builder.IsParallel);
  }

  [Fact]
  public void AsyncLambdaPreRuleConstructionThrowsOnNullOrEmpty()
  {
    Assert.Throws<ArgumentNullException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithPreRule((string)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithPreRule("")
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithPreRule("foo")
                .WithAction((Func<IEngineContext, TestInput, Task>)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithPreRule("foo")
                .WithAction((Func<IEngineContext, TestInput, CancellationToken, Task>)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithPreRule("foo")
                .WithPredicate((Func<IEngineContext, TestInput, Task<double>>)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithPreRule("foo")
                .WithPredicate((Func<IEngineContext, TestInput, CancellationToken, Task<double>>)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithPreRule("foo")
                .ThatProvides(null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithPreRule("foo")
                .ThatProvides("")
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithPreRule("foo")
                .ThatDependsOn("")
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithPreRule("foo")
                .ThatDependsOn((string)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            ProbabilisticEngineBuilder
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
            ProbabilisticEngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithRule((string)null)
);
    Assert.Throws<ArgumentNullException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>().WithRule("")
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithRule("foo")
                .WithAction((Func<IEngineContext, TestInput, TestOutput, Task>)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithRule("foo")
                .WithAction((Func<IEngineContext, TestInput, TestOutput, CancellationToken, Task>)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithRule("foo")
                .WithPredicate((Func<IEngineContext, TestInput, TestOutput, Task<double>>)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithRule("foo")
                .WithPredicate((Func<IEngineContext, TestInput, TestOutput, CancellationToken, Task<double>>)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>().WithRule("foo")
                .ThatProvides(null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>().WithRule("foo")
                .ThatProvides("")
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>().WithRule("foo")
                .ThatDependsOn("")
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>().WithRule("foo")
                .ThatDependsOn((string)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithRule("foo").ThatDependsOn((Type)null)
    );
  }

  [Fact]
  public async Task EmptyEngineConstruction()
  {
    var engine = ProbabilisticEngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
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
    var engine = ProbabilisticEngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>(logger)
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
    var engine = ProbabilisticEngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>(logger)
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
    var engine = ProbabilisticEngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                              .WithPostRule(new TestPostRule(1D))
                              .WithPostRule("test")
                              .WithPredicate((_, _) => Task.FromResult(1D))
                              .WithAction((_, _) => Task.CompletedTask)
                              .ThatProvides("foo")
                              .EndRule()
                              .WithPostRule("test2")
                              .WithPredicate((_, _, _) => Task.FromResult(1D))
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
    Assert.Equal(1D, await rule.DoesApply(null, null, default));
    rule = engine.PostRules.ElementAt(2);
    Assert.Contains(typeof(TestPostRule).FullName, rule.Dependencies);
    Assert.Contains("test", rule.Dependencies);
    Assert.Equal(1D, await rule.DoesApply(null, null, default));
    await engine.ApplyAsync(new TestInput(), new());
  }

  [Fact]
  public void LambdaPostRuleConstructionThrowsOnNullOrEmpty()
  {
    Assert.Throws<ArgumentException>(() =>
         ProbabilisticEngineBuilder
             .ForInputAndOutputAsync<TestInput, TestOutput>()
             .WithPostRule((string)null)
    );
    Assert.Throws<ArgumentException>(() =>
         ProbabilisticEngineBuilder
             .ForInputAndOutputAsync<TestInput, TestOutput>().WithPostRule("")
    );
    Assert.Throws<ArgumentNullException>(
      () =>
         ProbabilisticEngineBuilder
             .ForInputAndOutputAsync<TestInput, TestOutput>()
             .WithPostRule("foo")
             .WithAction((Func<IEngineContext, TestOutput, Task>)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
          ProbabilisticEngineBuilder
             .ForInputAndOutputAsync<TestInput, TestOutput>()
             .WithPostRule("foo")
             .WithAction((Func<IEngineContext, TestOutput, CancellationToken, Task>)null)
    );
    Assert.Throws<ArgumentNullException>(
      () =>
         ProbabilisticEngineBuilder
             .ForInputAndOutputAsync<TestInput, TestOutput>()
             .WithPostRule("foo")
             .WithPredicate((Func<IEngineContext, TestOutput, CancellationToken, Task<double>>)null)
    );
    Assert.Throws<ArgumentNullException>(
      () =>
         ProbabilisticEngineBuilder
             .ForInputAndOutputAsync<TestInput, TestOutput>()
             .WithPostRule("foo")
             .WithPredicate((Func<IEngineContext, TestOutput, Task<double>>)null)
    );
    Assert.Throws<ArgumentException>(
      () =>
        ProbabilisticEngineBuilder
            .ForInputAndOutputAsync<TestInput, TestOutput>()
            .WithPostRule("foo").ThatProvides(null)
    );
    Assert.Throws<ArgumentException>(
      () =>
        ProbabilisticEngineBuilder
            .ForInputAndOutputAsync<TestInput, TestOutput>()
            .WithPostRule("foo").ThatProvides("")
    );
    Assert.Throws<ArgumentException>(
      () =>
        ProbabilisticEngineBuilder
            .ForInputAndOutputAsync<TestInput, TestOutput>()
            .WithPostRule("foo").ThatDependsOn("")
    );
    Assert.Throws<ArgumentException>(
      () =>
        ProbabilisticEngineBuilder
            .ForInputAndOutputAsync<TestInput, TestOutput>()
            .WithPostRule("foo").ThatDependsOn((string)null)
    );
    Assert.Throws<ArgumentNullException>(
      () =>
        ProbabilisticEngineBuilder
            .ForInputAndOutputAsync<TestInput, TestOutput>()
            .WithPostRule("foo").ThatDependsOn((Type)null)
    );
  }

  [Fact]
  public async Task LambdaPreRuleConstruction()
  {
    var engine = ProbabilisticEngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                              .WithPreRule(new TestPreRule(1D))
                              .WithPreRule("test")
                              .WithPredicate((_, _) => Task.FromResult(1D))
                              .WithAction((_, _) => Task.CompletedTask)
                              .ThatProvides("foo")
                              .EndRule()
                              .WithPreRule("test2")
                              .WithPredicate((_, _, _) => Task.FromResult(1D))
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
    Assert.Equal(1D, await rule.DoesApply(null, null, default));
    rule = engine.PreRules.ElementAt(2);
    Assert.Contains("test", rule.Dependencies);
    Assert.Contains(typeof(TestPreRule).FullName, rule.Dependencies);
    Assert.Equal(1D, await rule.DoesApply(null, null, default));
    await engine.ApplyAsync(new TestInput(), new());
  }

  [Fact]
  public async Task LambdaRuleConstruction()
  {
    var engine = ProbabilisticEngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                              .WithRule(new TestRule(1D))
                              .WithRule("test")
                              .WithPredicate((_, _, _) => Task.FromResult(1D))
                              .WithAction((_, _, _) => Task.CompletedTask)
                              .ThatProvides("test1")
                              .EndRule()
                              .WithRule("test2")
                              .WithPredicate((_, _, _, _) => Task.FromResult(1D))
                              .WithAction((_, _, _, _) => Task.CompletedTask)
                              .ThatDependsOn("test1")
                              .ThatDependsOn(typeof(TestRule))
                              .EndRule()
                              .Build();
    Assert.Equal(3, engine.Rules.Count());
    var rule = engine.Rules.ElementAt(1);
    Assert.Equal("test", rule.Name);
    Assert.Contains("test1", rule.Provides);
    Assert.Equal(1D, await rule.DoesApply(null, null, null, default));
    rule = engine.Rules.ElementAt(2);
    Assert.Contains(typeof(TestRule).FullName, rule.Dependencies);
    Assert.Contains("test1", rule.Dependencies);
    Assert.Equal(1D, await rule.DoesApply(null, null, null, default));
    await engine.ApplyAsync(new TestInput(), new());
  }


  [Fact]
  public async Task RuleWrapping()
  {
    var engine = ProbabilisticEngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                              .WithPreRule(new TestRules.Probabilistic.TestPreRule(1D))
                              .WithRule(new TestRules.Probabilistic.TestRule(1D))
                              .WithPostRule(new TestRules.Probabilistic.TestPostRule(1D))
                              .Build();
    Assert.Single(engine.PreRules);
    var preRule = engine.PreRules.ElementAt(0);
    Assert.Equal($"{typeof(TestRules.Probabilistic.TestPreRule)} (wrapped async)", preRule.Name);
    Assert.Single(engine.Rules);
    var rule = engine.Rules.ElementAt(0);
    Assert.Equal($"{typeof(TestRules.Probabilistic.TestRule)} (wrapped async)", rule.Name);
    Assert.Single(engine.PostRules);
    var postRule = engine.PostRules.ElementAt(0);
    Assert.Equal($"{typeof(TestRules.Probabilistic.TestPostRule)} (wrapped async)", postRule.Name);
    Assert.Equal(1D, await preRule.DoesApply(null, null, default));
    Assert.Equal(1D, await rule.DoesApply(null, null, null, default));
    Assert.Equal(1D, await postRule.DoesApply(null, null, default));
  }

  [Fact]
  public async Task RuleWrappingMultiple()
  {
    var engine = ProbabilisticEngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                              .WithPreRules(new [] {new TestRules.Probabilistic.TestPreRule(1D)})
                              .WithRules(new [] {new TestRules.Probabilistic.TestRule(1D)})
                              .WithPostRules(new [] {new TestRules.Probabilistic.TestPostRule(1D)})
                              .Build();
    Assert.Single(engine.PreRules);
    var preRule = engine.PreRules.ElementAt(0);
    Assert.Equal($"{typeof(TestRules.Probabilistic.TestPreRule)} (wrapped async)", preRule.Name);
    Assert.Single(engine.Rules);
    var rule = engine.Rules.ElementAt(0);
    Assert.Equal($"{typeof(TestRules.Probabilistic.TestRule)} (wrapped async)", rule.Name);
    Assert.Single(engine.PostRules);
    var postRule = engine.PostRules.ElementAt(0);
    Assert.Equal($"{typeof(TestRules.Probabilistic.TestPostRule)} (wrapped async)", postRule.Name);
    Assert.Equal(1D, await preRule.DoesApply(null, null, default));
    Assert.Equal(1D, await rule.DoesApply(null, null, null, default));
    Assert.Equal(1D, await postRule.DoesApply(null, null, default));
  }

  [Fact]
  public void TypeAttributeDependency()
  {
    var engine = ProbabilisticEngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                              .WithPreRules(new [] {new DepTestAttrPreRule(1D), new DepTestAttrPreRule2(1D)})
                              .WithRules( new [] { new DepTestAttrRule(1D), new DepTestAttrRule2(1D)})
                              .WithPostRules(new [] { new DepTestAttrPostRule(1D), new DepTestAttrPostRule2(1D)})
                              .Build();
    Assert.NotNull(engine);
  }
}
