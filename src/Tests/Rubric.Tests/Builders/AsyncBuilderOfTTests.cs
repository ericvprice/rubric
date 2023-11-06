using Rubric.Builder;
using Rubric.Rules;
using Rubric.Tests.TestRules.Async;

namespace Rubric.Tests.Builders;

public class AsyncBuilderOfTTests
{
  [Fact]
  public void EmptyBuilder()
  {
    var builder = EngineBuilder.ForInputAsync<TestInput>();
    Assert.NotNull(builder.ExceptionHandler);
    Assert.NotNull(builder.Logger);
    Assert.False(builder.IsParallel);
  }

  [Fact]
  public void AsyncLambdaRuleConstructionThrowsOnNullOrEmpty()
  {
    Assert.Throws<ArgumentException>(
        () =>
            EngineBuilder
                .ForInputAsync<TestInput>()
                .WithRule((string)null)
    );
    Assert.Throws<ArgumentException>(
        () =>
            EngineBuilder
                .ForInputAsync<TestInput>().WithRule("")
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAsync<TestInput>()
                .WithRule("foo")
                .WithAction((Func<IEngineContext, TestInput, Task>)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAsync<TestInput>()
                .WithRule("foo")
                .WithAction((Func<IEngineContext, TestInput, CancellationToken, Task>)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAsync<TestInput>()
                .WithRule("foo")
                .WithPredicate((Func<IEngineContext, TestInput, Task<bool>>)null)
  );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAsync<TestInput>()
                .WithRule("foo")
                .WithPredicate((Func<IEngineContext, TestInput, CancellationToken, Task<bool>>)null)
  );
    Assert.Throws<ArgumentException>(
        () =>
            EngineBuilder
                .ForInputAsync<TestInput>().WithRule("foo")
                .ThatProvides(null)
    );
    Assert.Throws<ArgumentException>(
        () =>
            EngineBuilder
                .ForInputAsync<TestInput>().WithRule("foo")
                .ThatProvides("")
    );
    Assert.Throws<ArgumentException>(
        () =>
            EngineBuilder
                .ForInputAsync<TestInput>().WithRule("foo")
                .ThatDependsOn("")
    );
    Assert.Throws<ArgumentException>(
        () =>
            EngineBuilder
                .ForInputAsync<TestInput>().WithRule("foo")
                .ThatDependsOn((string)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
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
    //No rules is a valid engine... nothing should throw
    await engine.ApplyAsync(input);
    await engine.ApplyAsync(new[] { input });
    await engine.ApplyAsync(Array.Empty<TestInput>());
  }

  [Fact]
  public async Task EmptyEngineConstructionParallel()
  {
    var engine = EngineBuilder.ForInputAsync<TestInput>()
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
    var engine = EngineBuilder.ForInputAsync<TestInput>()
                              .AsParallel()
                              .Build();
    Assert.NotNull(engine);
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
    //Just assert nothing goes wrong with a blank engine
    await engine.ApplyAsync(input);
  }

  [Fact]
  public void EngineConstruction()
  {
    var logger = new TestLogger();
    var engine = EngineBuilder.ForInputAsync<TestInput>(logger)
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
    var engine = EngineBuilder.ForInputAsync<TestInput>()
                              .WithRule(new TestPreRule(true))
                              .WithRule("test")
                                  .WithPredicate((_, _) => Task.FromResult(true))
                                  .WithAction((_, _) => Task.CompletedTask)
                                  .ThatProvides("foo")
                              .EndRule()
                              .WithRule("test2")
                                  .WithPredicate((_, _, _) => Task.FromResult(true))
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
    Assert.True(await rule.DoesApply(null, null, default));
    rule = engine.Rules.ElementAt(2);
    Assert.Contains("test", rule.Dependencies);
    Assert.Contains(typeof(TestPreRule).FullName, rule.Dependencies);
    Assert.True(await rule.DoesApply(null, null, default));
    await engine.ApplyAsync(new TestInput());
  }

  [Fact]
  public async Task RuleWrapping()
  {
    var engine = EngineBuilder.ForInputAsync<TestInput>()
                              .WithRule(new TestRules.TestPreRule(true))
                              .Build();
    Assert.Single(engine.Rules);
    var rule = engine.Rules.ElementAt(0);
    Assert.Equal($"{typeof(TestRules.TestPreRule)} (wrapped async)", rule.Name);
    Assert.True(await rule.DoesApply(null, null, default));

  }

  [Fact]
  public async Task MultipleRuleWrapping()
  {
    var engine = EngineBuilder.ForInputAsync<TestInput>()
                              .WithRules(new [] {new TestRules.TestPreRule(true) })
                              .Build();
    Assert.Single(engine.Rules);
    var rule = engine.Rules.ElementAt(0);
    Assert.Equal($"{typeof(TestRules.TestPreRule)} (wrapped async)", rule.Name);
    Assert.True(await rule.DoesApply(null, null, default));

  }

  [Fact]
  public void TypeAttributeDependency()
  {
    var engine = EngineBuilder.ForInputAsync<TestInput>()
                              .WithRules(new Rubric.Rules.Async.IRule<TestInput>[] { new DepTestAttrPreRule(true), new DepTestAttrPreRule2(true) })
                              .Build();
    Assert.NotNull(engine);
  }

  [Fact]
  public void SyncWrapTypeAttributeDependency()
  {
    var engine = EngineBuilder.ForInputAsync<TestInput>()
                              .WithRules(new IRule<TestInput>[]
                              {
                                new TestRules.DepTestAttrPreRule(true),
                                new TestRules.DepTestAttrPreRule2(true)
                              })
                              .Build();
    Assert.NotNull(engine);
  }

}