using Rubric.Tests.TestRules.Probabilistic.Async;

namespace Rubric.Tests.Builders;

public class ProbabilisticAsyncBuilderOfTTests
{
  [Fact]
  public void EmptyBuilder()
  {
    var builder = ProbabilisticEngineBuilder.ForInputAsync<TestInput>();
    Assert.NotNull(builder.ExceptionHandler);
    Assert.NotNull(builder.Logger);
    Assert.False(builder.IsParallel);
  }

  [Fact]
  public void AsyncLambdaRuleConstructionThrowsOnNullOrEmpty()
  {
    Assert.Throws<ArgumentException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAsync<TestInput>()
                .WithRule((string)null)
    );
    Assert.Throws<ArgumentException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAsync<TestInput>().WithRule("")
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAsync<TestInput>()
                .WithRule("foo")
                .WithAction((Func<IEngineContext, TestInput, Task>)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAsync<TestInput>()
                .WithRule("foo")
                .WithAction((Func<IEngineContext, TestInput, CancellationToken, Task>)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAsync<TestInput>()
                .WithRule("foo")
                .WithPredicate((Func<IEngineContext, TestInput, Task<double>>)null)
  );
    Assert.Throws<ArgumentNullException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAsync<TestInput>()
                .WithRule("foo")
                .WithPredicate((Func<IEngineContext, TestInput, CancellationToken, Task<double>>)null)
  );
    Assert.Throws<ArgumentException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAsync<TestInput>().WithRule("foo")
                .ThatProvides(null)
    );
    Assert.Throws<ArgumentException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAsync<TestInput>().WithRule("foo")
                .ThatProvides("")
    );
    Assert.Throws<ArgumentException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAsync<TestInput>().WithRule("foo")
                .ThatDependsOn("")
    );
    Assert.Throws<ArgumentException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAsync<TestInput>().WithRule("foo")
                .ThatDependsOn((string)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAsync<TestInput>()
                .WithRule("foo").ThatDependsOn((Type)null)
    );
  }

  [Fact]
  public async Task EmptyEngineConstruction()
  {
    var engine = ProbabilisticEngineBuilder.ForInputAsync<TestInput>()
                              .Build();
    Assert.NotNull(engine);
    var input = new TestInput();
    //No rules is a valid engine... nothing should throw
    await engine.ApplyAsync(input);
    await engine.ApplyAsync(new[] { input });
    await engine.ApplyAsync(Array.Empty<TestInput>());
  }

  [Fact]
  public async Task EmptyEngineConstructionParallel()
  {
    var engine = ProbabilisticEngineBuilder.ForInputAsync<TestInput>()
                              .AsParallel()
                              .Build();
    Assert.NotNull(engine);
    var input = new TestInput();
    //No rules is a valid engine... nothing should throw
    await engine.ApplyAsync(input);
    await engine.ApplyAsync(new[] { input });
    await engine.ApplyAsync(Array.Empty<TestInput>());
  }

  [Fact]
  public void AsParallel()
  {
    var engine = ProbabilisticEngineBuilder.ForInputAsync<TestInput>()
                              .AsParallel()
                              .Build();
    Assert.NotNull(engine);
    Assert.True(engine.IsParallel);
  }

  [Fact]
  public async Task EmptyEngineConstructionWithLogger()
  {
    var logger = new TestLogger();
    var engine = ProbabilisticEngineBuilder.ForInputAsync<TestInput>(logger)
                              .Build();
    Assert.NotNull(engine);
    Assert.Equal(logger, engine.Logger);
    var input = new TestInput();
    //Just assert nothing goes wrong with a blank engine
    await engine.ApplyAsync(input);
  }

  [Fact]
  public void EngineConstruction()
  {
    var logger = new TestLogger();
    var engine = ProbabilisticEngineBuilder.ForInputAsync<TestInput>(logger)
                              .WithRule(new TestDefaultPreRule())
                              .WithExceptionHandler(ExceptionHandlers.Ignore)
                              .AsParallel()
                              .Build();
    Assert.NotNull(engine);
    Assert.Equal(logger, engine.Logger);
    Assert.Single(engine.Rules);
    Assert.Equal(ExceptionHandlers.Ignore, engine.ExceptionHandler);
    Assert.True(engine.IsParallel);
  }



  [Fact]
  public async Task LambdaRuleConstruction()
  {
    var engine = ProbabilisticEngineBuilder.ForInputAsync<TestInput>()
                              .WithRule(new TestPreRule(1D))
                              .WithRule("test")
                                  .WithPredicate((_, _) => Task.FromResult(1D))
                                  .WithAction((_, _) => Task.CompletedTask)
                                  .ThatProvides("foo")
                              .EndRule()
                              .WithRule("test2")
                                  .WithPredicate((_, _, _) => Task.FromResult(1D))
                                  .WithAction((_, _, _) => Task.CompletedTask)
                                  .ThatDependsOn(typeof(TestPreRule))
                                  .ThatDependsOn("test")
                              .EndRule()
                              .Build();
    Assert.Equal(3, engine.Rules.Count());
    var rule = engine.Rules.ElementAt(1);
    Assert.Equal("test", rule.Name);
    Assert.Contains("foo", rule.Provides);
    Assert.Contains("test", rule.Provides);
    Assert.Equal(1D, await rule.DoesApply(null, null, default));
    rule = engine.Rules.ElementAt(2);
    Assert.Contains("test", rule.Dependencies);
    Assert.Contains(typeof(TestPreRule).FullName, rule.Dependencies);
    Assert.Equal(1D, await rule.DoesApply(null, null, default));
    await engine.ApplyAsync(new TestInput());
  }

  [Fact]
  public async Task RuleWrapping()
  {
    var engine = ProbabilisticEngineBuilder.ForInputAsync<TestInput>()
                              .WithRule(new TestRules.Probabilistic.TestPreRule(1D))
                              .Build();
    Assert.Single(engine.Rules);
    var rule = engine.Rules.ElementAt(0);
    Assert.Equal($"{typeof(TestRules.Probabilistic.TestPreRule)} (wrapped async)", rule.Name);
    Assert.Equal(1D, await rule.DoesApply(null, null, default));

  }

  [Fact]
  public async Task MultipleRuleWrapping()
  {
    var engine = ProbabilisticEngineBuilder.ForInputAsync<TestInput>()
                              .WithRules(new [] {new TestRules.Probabilistic.TestPreRule(1D) })
                              .Build();
    Assert.Single(engine.Rules);
    var rule = engine.Rules.ElementAt(0);
    Assert.Equal($"{typeof(TestRules.Probabilistic.TestPreRule)} (wrapped async)", rule.Name);
    Assert.Equal(1D, await rule.DoesApply(null, null, default));

  }

  [Fact]
  public void TypeAttributeDependency()
  {
    var engine = ProbabilisticEngineBuilder
                    .ForInputAsync<TestInput>()
                    .WithRules(new [] { new DepTestAttrPreRule(1D), new DepTestAttrPreRule2(1D) })
                    .Build();
    Assert.NotNull(engine);
  }

  [Fact]
  public void SyncWrapTypeAttributeDependency()
  {
    var engine = ProbabilisticEngineBuilder
                   .ForInputAsync<TestInput>()
                   .WithRules(new Rubric.Rules.Probabilistic.Rule<TestInput>[]
                   {
                     new TestRules.Probabilistic.DepTestAttrPreRule(1D),
                     new TestRules.Probabilistic.DepTestAttrPreRule2(1D)
                   })
                   .Build();
    Assert.NotNull(engine);
  }
}