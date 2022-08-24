using Rubric.Tests.TestRules;
using Rubric.Tests.TestRules.Async;
using Rubric.Tests.TestRules.DepTestTypeAttribute;

namespace Rubric.Tests.Builders;

public class AsyncBuilderOfTInTOutTests
{
  [Fact]
  public void AsyncLambdaPreRuleConstructionThrowsOnNullOrEmpty()
  {
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithAsyncPreRule((string)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithAsyncPreRule("")
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithAsyncPreRule("foo")
                .WithAction((Func<IEngineContext, TestInput, Task>)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithAsyncPreRule("foo")
                .WithAction((Func<IEngineContext, TestInput, CancellationToken, Task>)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithAsyncPreRule("foo")
                .WithPredicate((Func<IEngineContext, TestInput, Task<bool>>)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithAsyncPreRule("foo")
                .WithPredicate((Func<IEngineContext, TestInput, CancellationToken, Task<bool>>)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithAsyncPreRule("foo")
                .ThatProvides(null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithAsyncPreRule("foo")
                .ThatProvides("")
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithAsyncPreRule("foo")
                .ThatDependsOn("")
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithAsyncPreRule("foo")
                .ThatDependsOn((string)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithAsyncPreRule("foo")
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
                .WithAsyncRule((string)null)
);
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>().WithAsyncRule("")
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithAsyncRule("foo")
                .WithAction((Func<IEngineContext, TestInput, TestOutput, Task>)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithAsyncRule("foo")
                .WithAction((Func<IEngineContext, TestInput, TestOutput, CancellationToken, Task>)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithAsyncRule("foo")
                .WithPredicate((Func<IEngineContext, TestInput, TestOutput, Task<bool>>)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithAsyncRule("foo")
                .WithPredicate((Func<IEngineContext, TestInput, TestOutput, CancellationToken, Task<bool>>)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>().WithAsyncRule("foo")
                .ThatProvides(null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>().WithAsyncRule("foo")
                .ThatProvides("")
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>().WithAsyncRule("foo")
                .ThatDependsOn("")
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>().WithAsyncRule("foo")
                .ThatDependsOn((string)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutputAsync<TestInput, TestOutput>()
                .WithAsyncRule("foo").ThatDependsOn((Type)null)
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
                              .WithAsyncPreRule(new TestDefaultAsyncPreRule())
                              .WithAsyncRule(new TestDefaultAsyncRule())
                              .WithAsyncPostRule(new TestDefaultAsyncPostRule())
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
                              .WithAsyncPostRule(new TestAsyncPostRule(true))
                              .WithAsyncPostRule("test")
                              .WithPredicate((_, _) => Task.FromResult(true))
                              .WithAction((_, _) => Task.CompletedTask)
                              .ThatProvides("foo")
                              .EndRule()
                              .WithAsyncPostRule("test2")
                              .WithPredicate((_, _, _) => Task.FromResult(true))
                              .WithAction((_, _, _) => Task.CompletedTask)
                              .ThatDependsOn(typeof(TestAsyncPostRule))
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
    Assert.Contains(typeof(TestAsyncPostRule).FullName, rule.Dependencies);
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
             .WithAsyncPostRule((string)null)
    );
    Assert.Throws<ArgumentException>(() =>
         EngineBuilder
             .ForInputAndOutputAsync<TestInput, TestOutput>().WithAsyncPostRule("")
    );
    Assert.Throws<ArgumentNullException>(
      () =>
         EngineBuilder
             .ForInputAndOutputAsync<TestInput, TestOutput>()
             .WithAsyncPostRule("foo")
             .WithAction((Func<IEngineContext, TestOutput, Task>)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
          EngineBuilder
             .ForInputAndOutputAsync<TestInput, TestOutput>()
             .WithAsyncPostRule("foo")
             .WithAction((Func<IEngineContext, TestOutput, CancellationToken, Task>)null)
    );
    Assert.Throws<ArgumentNullException>(
      () =>
         EngineBuilder
             .ForInputAndOutputAsync<TestInput, TestOutput>()
             .WithAsyncPostRule("foo")
             .WithPredicate((Func<IEngineContext, TestOutput, CancellationToken, Task<bool>>)null)
    );
    Assert.Throws<ArgumentNullException>(
      () =>
         EngineBuilder
             .ForInputAndOutputAsync<TestInput, TestOutput>()
             .WithAsyncPostRule("foo")
             .WithPredicate((Func<IEngineContext, TestOutput, Task<bool>>)null)
    );
    Assert.Throws<ArgumentException>(
      () =>
        EngineBuilder
            .ForInputAndOutputAsync<TestInput, TestOutput>()
            .WithAsyncPostRule("foo").ThatProvides(null)
    );
    Assert.Throws<ArgumentException>(
      () =>
        EngineBuilder
            .ForInputAndOutputAsync<TestInput, TestOutput>()
            .WithAsyncPostRule("foo").ThatProvides("")
    );
    Assert.Throws<ArgumentException>(
      () =>
        EngineBuilder
            .ForInputAndOutputAsync<TestInput, TestOutput>()
            .WithAsyncPostRule("foo").ThatDependsOn("")
    );
    Assert.Throws<ArgumentException>(
      () =>
        EngineBuilder
            .ForInputAndOutputAsync<TestInput, TestOutput>()
            .WithAsyncPostRule("foo").ThatDependsOn((string)null)
    );
    Assert.Throws<ArgumentNullException>(
      () =>
        EngineBuilder
            .ForInputAndOutputAsync<TestInput, TestOutput>()
            .WithAsyncPostRule("foo").ThatDependsOn((Type)null)
    );
  }

  [Fact]
  public async Task LambdaPreRuleConstruction()
  {
    var engine = EngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                              .WithAsyncPreRule(new TestAsyncPreRule(true))
                              .WithAsyncPreRule("test")
                              .WithPredicate((_, _) => Task.FromResult(true))
                              .WithAction((_, _) => Task.CompletedTask)
                              .ThatProvides("foo")
                              .EndRule()
                              .WithAsyncPreRule("test2")
                              .WithPredicate((_, _, _) => Task.FromResult(true))
                              .WithAction((_, _, _) => Task.CompletedTask)
                              .ThatDependsOn(typeof(TestAsyncPreRule))
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
    Assert.Contains(typeof(TestAsyncPreRule).FullName, rule.Dependencies);
    Assert.True(await rule.DoesApply(null, null, default));
    await engine.ApplyAsync(new TestInput(), new());
  }

  [Fact]
  public async Task LambdaRuleConstruction()
  {
    var engine = EngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                              .WithAsyncRule(new TestAsyncRule(true))
                              .WithAsyncRule("test")
                              .WithPredicate((_, _, _) => Task.FromResult(true))
                              .WithAction((_, _, _) => Task.CompletedTask)
                              .ThatProvides("test1")
                              .EndRule()
                              .WithAsyncRule("test2")
                              .WithPredicate((_, _, _, _) => Task.FromResult(true))
                              .WithAction((_, _, _, _) => Task.CompletedTask)
                              .ThatDependsOn("test1")
                              .ThatDependsOn(typeof(TestAsyncRule))
                              .EndRule()
                              .Build();
    Assert.Equal(3, engine.Rules.Count());
    var rule = engine.Rules.ElementAt(1);
    Assert.Equal("test", rule.Name);
    Assert.Contains("test1", rule.Provides);
    Assert.True(await rule.DoesApply(null, null, null, default));
    rule = engine.Rules.ElementAt(2);
    Assert.Contains(typeof(TestAsyncRule).FullName, rule.Dependencies);
    Assert.Contains("test1", rule.Dependencies);
    Assert.True(await rule.DoesApply(null, null, null, default));
    await engine.ApplyAsync(new TestInput(), new());
  }


  [Fact]
  public async Task RuleWrapping()
  {
    var engine = EngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                              .WithPreRule(new TestPreRule(true))
                              .WithRule(new TestRule(true))
                              .WithAsyncPostRule(new TestPostRule(true))
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
    Assert.True(await preRule.DoesApply(null, null, default));
    Assert.True(await rule.DoesApply(null, null, null, default));
    Assert.True(await postRule.DoesApply(null, null, default));
  }

  [Fact]
  public void TypeAttributeDependency()
  {
    var engine = EngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                              .WithAsyncPreRule(new TestRules.DepTestTypeAttribute.DepTestAsyncPreRule(true))
                              .WithAsyncPreRule(new DepTestAsyncPreRule2(true))
                              .WithAsyncRule(new TestRules.DepTestTypeAttribute.DepTestAsyncRule(true))
                              .WithAsyncRule(new DepTestAsyncRule2(true))
                              .WithAsyncPostRule(new TestRules.DepTestTypeAttribute.DepTestAsyncPostRule(true))
                              .WithAsyncPostRule(new DepTestAsyncPostRule2(true))
                              .Build();
    Assert.NotNull(engine);
  }
}
