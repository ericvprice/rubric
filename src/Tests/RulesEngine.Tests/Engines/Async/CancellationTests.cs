namespace RulesEngine.Tests.Engines.Async;


public class CancellationTests
{

  [Fact]
  public async Task SingleEngineParallelCancellation()
  {
    var engine = EngineBuilder.ForInputAsync<TestInput>()
                              .WithRule("1")
                                .WithAction(async (c, i, t) =>
                                {
                                  await Task.Delay(100, t);
                                  i.Counter++;
                                })
                              .EndRule()
                              .WithRule("2")
                                .WithAction(async (c, i, t) =>
                                {
                                  await Task.Delay(200, t);
                                  i.Counter++;
                                })
                              .EndRule()
                              .WithRule("3")
                                .WithAction(async (c, i, t) =>
                                {
                                  await Task.Delay(300, t);
                                  i.Counter++;
                                })
                              .EndRule()
                              .AsParallel()
                              .Build();
    var cts = new CancellationTokenSource();
    var input = new TestInput();
    var task = engine.ApplyAsync(input, null, cts.Token);
    await Task.Delay(150);
    cts.Cancel();
    //We should get an OperationCancelledException bubble up.
    var exception = await Assert.ThrowsAnyAsync<OperationCanceledException>(() => task);
    Assert.Null(engine.LastException);
    Assert.Equal(1, input.Counter);
  }

  [Fact]
  public async Task SingleEngineCancellation()
  {
    var engine = EngineBuilder.ForInputAsync<TestInput>()
                              .WithRule("1")
                                .WithAction(async (c, i, t) =>
                                {
                                  await Task.Delay(100, t);
                                  i.Counter++;
                                })
                              .EndRule()
                              .WithRule("2")
                                .WithAction(async (c, i, t) =>
                                {
                                  await Task.Delay(200, t);
                                  i.Counter++;
                                })
                              .EndRule()
                              .WithRule("3")
                                .WithAction(async (c, i, t) =>
                                {
                                  await Task.Delay(300, t);
                                  i.Counter++;
                                })
                              .EndRule()
                              .Build();
    var cts = new CancellationTokenSource();
    var input = new TestInput();
    var task = engine.ApplyAsync(input, null, cts.Token);
    await Task.Delay(150);
    cts.Cancel();
    //We should get an OperationCancelledException bubble up.
    var exception = await Assert.ThrowsAnyAsync<OperationCanceledException>(() => task);
    Assert.Null(engine.LastException);
    Assert.Equal(1, input.Counter);
  }

  [Fact]
  public async Task EngineParallelPreCancellation()
  {
    var engine = EngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                              .WithPreRule("1")
                                .WithAction(async (c, i, t) =>
                                {
                                  await Task.Delay(100, t);
                                  i.Counter++;
                                })
                              .EndRule()
                              .WithPreRule("2")
                                .WithAction(async (c, i, t) =>
                                {
                                  await Task.Delay(200, t);
                                  i.Counter++;
                                })
                              .EndRule()
                              .WithPreRule("3")
                                .WithAction(async (c, i, t) =>
                                {
                                  await Task.Delay(300, t);
                                  i.Counter++;
                                })
                              .EndRule()
                              .AsParallel()
                              .Build();
    var cts = new CancellationTokenSource();
    var input = new TestInput();
    var output = new TestOutput();
    var task = engine.ApplyAsync(input, output, null, cts.Token);
    await Task.Delay(150);
    cts.Cancel();
    //We should get an OperationCancelledException bubble up.
    var exception = await Assert.ThrowsAnyAsync<OperationCanceledException>(() => task);
    Assert.Null(engine.LastException);
    Assert.Equal(1, input.Counter);
  }

  [Fact]
  public async Task EnginePreCancellation()
  {
    var engine = EngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                              .WithPreRule("1")
                                .WithAction(async (c, i, t) =>
                                {
                                  await Task.Delay(100, t);
                                  i.Counter++;
                                })
                              .EndRule()
                              .WithPreRule("2")
                                .WithAction(async (c, i, t) =>
                                {
                                  await Task.Delay(200, t);
                                  i.Counter++;
                                })
                              .EndRule()
                              .WithPreRule("3")
                                .WithAction(async (c, i, t) =>
                                {
                                  await Task.Delay(300, t);
                                  i.Counter++;
                                })
                              .EndRule()
                              .Build();
    var cts = new CancellationTokenSource();
    var input = new TestInput();
    var output = new TestOutput();
    var task = engine.ApplyAsync(input, output, null, cts.Token);
    await Task.Delay(150);
    cts.Cancel();
    //We should get an OperationCancelledException bubble up.
    var exception = await Assert.ThrowsAnyAsync<OperationCanceledException>(() => task);
    Assert.Null(engine.LastException);
    Assert.Equal(1, input.Counter);
  }

  [Fact]
  public async Task ParallelEngineCancellation()
  {
    var engine = EngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                              .WithRule("1")
                                .WithAction(async (c, i, o, t) =>
                                {
                                  await Task.Delay(100, t);
                                  i.Counter++;
                                  o.Counter++;
                                })
                              .EndRule()
                              .WithRule("2")
                                .WithAction(async (c, i, o, t) =>
                                {
                                  await Task.Delay(200, t);
                                  i.Counter++;
                                  o.Counter++;
                                })
                              .EndRule()
                              .WithRule("3")
                                .WithAction(async (c, i, o, t) =>
                                {
                                  await Task.Delay(300, t);
                                  i.Counter++;
                                  o.Counter++;
                                })
                              .EndRule()
                              .AsParallel()
                              .Build();
    var cts = new CancellationTokenSource();
    var input = new TestInput();
    var output = new TestOutput();
    var task = engine.ApplyAsync(input, output, null, cts.Token);
    await Task.Delay(150);
    cts.Cancel();
    //We should get an OperationCancelledException bubble up.
    var exception = await Assert.ThrowsAnyAsync<OperationCanceledException>(() => task);
    Assert.Null(engine.LastException);
    Assert.Equal(1, input.Counter);
    Assert.Equal(1, output.Counter);
  }

  [Fact]
  public async Task EngineCancellation()
  {
    var engine = EngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                              .WithRule("1")
                                .WithAction(async (c, i, o, t) =>
                                {
                                  await Task.Delay(100, t);
                                  i.Counter++;
                                  o.Counter++;
                                })
                              .EndRule()
                              .WithRule("2")
                                .WithAction(async (c, i, o, t) =>
                                {
                                  await Task.Delay(200, t);
                                  i.Counter++;
                                  o.Counter++;
                                })
                              .EndRule()
                              .WithRule("3")
                                .WithAction(async (c, i, o, t) =>
                                {
                                  await Task.Delay(300, t);
                                  i.Counter++;
                                  o.Counter++;
                                })
                              .EndRule()
                              .Build();
    var cts = new CancellationTokenSource();
    var input = new TestInput();
    var output = new TestOutput();
    var task = engine.ApplyAsync(input, output, null, cts.Token);
    await Task.Delay(100);
    cts.Cancel();
    //We should get an OperationCancelledException bubble up.
    var exception = await Assert.ThrowsAnyAsync<OperationCanceledException>(() => task);
    Assert.Null(engine.LastException);
    Assert.Equal(1, input.Counter);
    Assert.Equal(1, output.Counter);
  }

  [Fact]
  public async Task EngineParallelPostCancellation()
  {
    var engine = EngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                              .WithPostRule("1")
                                .WithAction(async (c, o, t) =>
                                {
                                  await Task.Delay(100, t);
                                  o.Counter++;
                                })
                              .EndRule()
                              .WithPostRule("2")
                                .WithAction(async (c, o, t) =>
                                {
                                  await Task.Delay(200, t);
                                  o.Counter++;
                                })
                              .EndRule()
                              .WithPostRule("3")
                                .WithAction(async (c, o, t) =>
                                {
                                  await Task.Delay(300, t);
                                  o.Counter++;
                                })
                              .EndRule()
                              .AsParallel()
                              .Build();
    var cts = new CancellationTokenSource();
    var input = new TestInput();
    var output = new TestOutput();
    var task = engine.ApplyAsync(input, output, null, cts.Token);
    await Task.Delay(150);
    cts.Cancel();
    //We should get an OperationCancelledException bubble up.
    var exception = await Assert.ThrowsAnyAsync<OperationCanceledException>(() => task);
    Assert.Null(engine.LastException);
    Assert.Equal(1, output.Counter);
  }

  [Fact]
  public async Task EnginePostCancellation()
  {
    var engine = EngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                              .WithPostRule("1")
                                .WithAction(async (c, o, t) =>
                                {
                                  await Task.Delay(100, t);
                                  o.Counter++;
                                })
                              .EndRule()
                              .WithPostRule("2")
                                .WithAction(async (c, o, t) =>
                                {
                                  await Task.Delay(200, t);
                                  o.Counter++;
                                })
                              .EndRule()
                              .WithPostRule("3")
                                .WithAction(async (c, o, t) =>
                                {
                                  await Task.Delay(300, t);
                                  o.Counter++;
                                })
                              .EndRule()
                              .Build();
    var cts = new CancellationTokenSource();
    var input = new TestInput();
    var output = new TestOutput();
    var task = engine.ApplyAsync(input, output, null, cts.Token);
    await Task.Delay(150);
    cts.Cancel();
    //We should get an OperationCancelledException bubble up.
    var exception = await Assert.ThrowsAnyAsync<OperationCanceledException>(() => task);
    Assert.Null(engine.LastException);
    Assert.Equal(1, output.Counter);
  }

}

