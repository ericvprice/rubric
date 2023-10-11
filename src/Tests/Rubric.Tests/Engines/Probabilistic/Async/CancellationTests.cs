// ReSharper disable MethodSupportsCancellation

namespace Rubric.Tests.Engines.Probabilistic.Async;

public class CancellationTests
{

  [Fact]
  public async Task SingleEngineParallelCancellation()
  {
    var engine = EngineBuilder.ForInputAsync<TestInput>()
                              .WithRule("1")
                                .WithAction(async (_, i, t) =>
                                {
                                  await Task.Delay(100, t);
                                  i.Counter++;
                                })
                              .EndRule()
                              .WithRule("2")
                                .WithAction(async (_, i, t) =>
                                {
                                  await Task.Delay(200, t);
                                  i.Counter++;
                                })
                              .EndRule()
                              .WithRule("3")
                                .WithAction(async (_, i, t) =>
                                {
                                  await Task.Delay(300, t);
                                  i.Counter++;
                                })
                              .EndRule()
                              .AsParallel()
                              .Build();
    var cts = new CancellationTokenSource();
    var input = new TestInput();
    var context = new EngineContext();
    var task = engine.ApplyAsync(input, context, cts.Token);
    await Task.Delay(150);
    cts.Cancel();
    //We should get an OperationCancelledException bubble up.
    await Assert.ThrowsAnyAsync<OperationCanceledException>(() => task);
    Assert.Null(context.GetLastException());
    Assert.True(input.Counter < 2);
  }

  [Fact]
  public async Task SingleEngineCancellation()
  {
    var engine = EngineBuilder.ForInputAsync<TestInput>()
                              .WithRule("1")
                                .WithAction(async (_, i, t) =>
                                {
                                  await Task.Delay(100, t);
                                  i.Counter++;
                                })
                              .EndRule()
                              .WithRule("2")
                                .WithAction(async (_, i, t) =>
                                {
                                  await Task.Delay(200, t);
                                  i.Counter++;
                                })
                              .EndRule()
                              .WithRule("3")
                                .WithAction(async (_, i, t) =>
                                {
                                  await Task.Delay(300, t);
                                  i.Counter++;
                                })
                              .EndRule()
                              .Build();
    var cts = new CancellationTokenSource();
    var input = new TestInput();
    var context = new EngineContext();
    var task = engine.ApplyAsync(input, context, cts.Token);
    await Task.Delay(150);
    cts.Cancel();
    //We should get an OperationCancelledException bubble up.
    await Assert.ThrowsAnyAsync<OperationCanceledException>(() => task);
    Assert.Null(context.GetLastException());
    Assert.Equal(1, input.Counter);
  }

  [Fact]
  public async Task EngineParallelPreCancellation()
  {
    var engine = EngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                              .WithAsyncPreRule("1")
                                .WithAction(async (_, i, t) =>
                                {
                                  await Task.Delay(100, t);
                                  i.Counter++;
                                })
                              .EndRule()
                              .WithAsyncPreRule("2")
                                .WithAction(async (_, i, t) =>
                                {
                                  await Task.Delay(200, t);
                                  i.Counter++;
                                })
                              .EndRule()
                              .WithAsyncPreRule("3")
                                .WithAction(async (_, i, t) =>
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
    var context = new EngineContext();
    var task = engine.ApplyAsync(input, output, context, cts.Token);
    await Task.Delay(150);
    cts.Cancel();
    //We should get an OperationCancelledException bubble up.
    await Assert.ThrowsAnyAsync<OperationCanceledException>(() => task);
    Assert.Null(context.GetLastException());
    Assert.Equal(1, input.Counter);
  }

  [Fact]
  public async Task EnginePreCancellation()
  {
    var engine = EngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                              .WithAsyncPreRule("1")
                                .WithAction(async (_, i, t) =>
                                {
                                  await Task.Delay(100, t);
                                  i.Counter++;
                                })
                              .EndRule()
                              .WithAsyncPreRule("2")
                                .WithAction(async (_, i, t) =>
                                {
                                  await Task.Delay(200, t);
                                  i.Counter++;
                                })
                              .EndRule()
                              .WithAsyncPreRule("3")
                                .WithAction(async (_, i, t) =>
                                {
                                  await Task.Delay(300, t);
                                  i.Counter++;
                                })
                              .EndRule()
                              .Build();
    var cts = new CancellationTokenSource();
    var input = new TestInput();
    var output = new TestOutput();
    var context = new EngineContext();
    var task = engine.ApplyAsync(input, output, context, cts.Token);
    await Task.Delay(150);
    cts.Cancel();
    //We should get an OperationCancelledException bubble up.
    await Assert.ThrowsAnyAsync<OperationCanceledException>(() => task);
    Assert.Null(context.GetLastException());
    Assert.Equal(1, input.Counter);
  }

  [Fact]
  public async Task ParallelEngineCancellation()
  {
    var engine = EngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                              .WithAsyncRule("1")
                                .WithAction(async (_, i, o, t) =>
                                {
                                  await Task.Delay(100, t);
                                  i.Counter++;
                                  o.Counter++;
                                })
                              .EndRule()
                              .WithAsyncRule("2")
                                .WithAction(async (_, i, o, t) =>
                                {
                                  await Task.Delay(200, t);
                                  i.Counter++;
                                  o.Counter++;
                                })
                              .EndRule()
                              .WithAsyncRule("3")
                                .WithAction(async (_, i, o, t) =>
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
    var context = new EngineContext();
    var task = engine.ApplyAsync(input, output, context, cts.Token);
    await Task.Delay(150);
    cts.Cancel();
    //We should get an OperationCancelledException bubble up.
    await Assert.ThrowsAnyAsync<OperationCanceledException>(() => task);
    Assert.Null(context.GetLastException());
    Assert.Equal(1, input.Counter);
    Assert.Equal(1, output.Counter);
  }

  [Fact]
  public async Task EngineCancellation()
  {
    var engine = EngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                              .WithAsyncRule("1")
                                .WithAction(async (_, i, o, t) =>
                                {
                                  await Task.Delay(100, t);
                                  i.Counter++;
                                  o.Counter++;
                                })
                              .EndRule()
                              .WithAsyncRule("2")
                                .WithAction(async (_, i, o, t) =>
                                {
                                  await Task.Delay(200, t);
                                  i.Counter++;
                                  o.Counter++;
                                })
                              .EndRule()
                              .WithAsyncRule("3")
                                .WithAction(async (_, i, o, t) =>
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
    var context = new EngineContext();
    var task = engine.ApplyAsync(input, output, context, cts.Token);
    await Task.Delay(150);
    cts.Cancel();
    //We should get an OperationCancelledException bubble up.
    await Assert.ThrowsAnyAsync<OperationCanceledException>(() => task);
    Assert.Null(context.GetLastException());
    Assert.Equal(1, input.Counter);
    Assert.Equal(1, output.Counter);
  }

  [Fact]
  public async Task EngineParallelPostCancellation()
  {
    var engine = EngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                              .WithAsyncPostRule("1")
                                .WithAction(async (_, o, t) =>
                                {
                                  await Task.Delay(100, t);
                                  o.Counter++;
                                })
                              .EndRule()
                              .WithAsyncPostRule("2")
                                .WithAction(async (_, o, t) =>
                                {
                                  await Task.Delay(200, t);
                                  o.Counter++;
                                })
                              .EndRule()
                              .WithAsyncPostRule("3")
                                .WithAction(async (_, o, t) =>
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
    var context = new EngineContext();
    var task = engine.ApplyAsync(input, output, context, cts.Token);
    await Task.Delay(150);
    cts.Cancel();
    //We should get an OperationCancelledException bubble up.
    await Assert.ThrowsAnyAsync<OperationCanceledException>(() => task);
    Assert.Null(context.GetLastException());
    Assert.Equal(1, output.Counter);
  }

  [Fact]
  public async Task EnginePostCancellation()
  {
    var engine = EngineBuilder.ForInputAndOutputAsync<TestInput, TestOutput>()
                              .WithAsyncPostRule("1")
                                .WithAction(async (_, o, t) =>
                                {
                                  await Task.Delay(100, t);
                                  o.Counter++;
                                })
                              .EndRule()
                              .WithAsyncPostRule("2")
                                .WithAction(async (_, o, t) =>
                                {
                                  await Task.Delay(200, t);
                                  o.Counter++;
                                })
                              .EndRule()
                              .WithAsyncPostRule("3")
                                .WithAction(async (_, o, t) =>
                                {
                                  await Task.Delay(300, t);
                                  o.Counter++;
                                })
                              .EndRule()
                              .Build();
    var cts = new CancellationTokenSource();
    var input = new TestInput();
    var output = new TestOutput();
    var context = new EngineContext();
    var task = engine.ApplyAsync(input, output, context, cts.Token);
    await Task.Delay(150);
    cts.Cancel();
    //We should get an OperationCancelledException bubble up.
    await Assert.ThrowsAnyAsync<OperationCanceledException>(() => task);
    Assert.Null(context.GetLastException());
    Assert.Equal(1, output.Counter);
  }

}
