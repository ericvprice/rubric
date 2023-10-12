﻿using Rubric.Rulesets.Async;
using Rubric.Engines.Async.Default;

namespace Rubric.Tests.Core;

public class AsyncContextExtensionTests { 
  [Fact]
  public void AsycContextExtensions()
  {
    IEngineContext ctx = new EngineContext();
    var logger = new TestLogger();
    var engine = new RuleEngine<TestInput, TestOutput>(new Ruleset<TestInput, TestOutput>(), false, null, logger);
    engine.Reset(ctx);
    Assert.Equal(engine, ctx.GetEngine());
    Assert.Equal(engine, ctx.GetAsyncEngine<TestInput, TestOutput>());
    Assert.Equal(logger, ctx.GetLogger());
    Assert.True(ctx.IsAsync());
    Assert.False(ctx.IsParallel());
    engine.IsParallel = true;
    Assert.True(ctx.IsParallel());
    Assert.Equal(typeof(TestInput), ctx.GetInputType());
    Assert.Equal(typeof(TestOutput), ctx.GetOutputType());
  }
}