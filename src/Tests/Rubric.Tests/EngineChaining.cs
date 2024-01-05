using Rubric.Builder;
using Rubric.Engines.Implementation;

namespace Rubric.Tests;

public class EngineChainingTests
{
  [Fact]
  public void EngineChaining()
  {
    var engine = EngineBuilder.ForInputAndOutput<TestInput, List<string>>()
                              .WithRule("first")
                              .WithAction((_, _, s) => s.Add("test"))
                              .EndRule()
                              .Build();
    var engine2 = EngineBuilder.ForInputAndOutput<List<string>, TestOutput>()
                               .WithRule("second")
                               .WithAction((_, i, o) =>
                               {
                                 foreach (var s in i)
                                   o.Outputs.Add(s);
                               })
                               .EndRule()
                               .Build();
    var chained = engine.Chain(() => new(), engine2);
    Assert.IsType<ChainedEngine<TestInput, List<string>, TestOutput>>(chained);
    var typed = (ChainedEngine<TestInput, List<string>, TestOutput>)chained;
    Assert.Equal(engine, typed.First);
    Assert.Equal(engine2, typed.Second);
    Assert.Throws<NotImplementedException>(() => chained.Logger);
    Assert.Throws<NotImplementedException>(() => chained.ExceptionHandler);
    Assert.Equal(typeof(TestInput), chained.InputType);
    Assert.Equal(typeof(TestOutput), chained.OutputType);
    Assert.Equal(engine.PreRules, chained.PreRules);
    Assert.Equal(engine2.PostRules, chained.PostRules);
    Assert.Throws<NotImplementedException>(() => chained.Rules);
    Assert.False(chained.IsAsync);
    var output = new TestOutput();
    var input = new TestInput();
    chained.Apply(input, output);
    Assert.Contains("test", output.Outputs);
    output.Outputs.Clear();
    var inputs = new[] { input, new TestInput() };
    chained.Apply(inputs, output);
    Assert.Contains("test", output.Outputs);
    Assert.Equal(2, output.Outputs.Count);
  }

  [Fact]
  public void ChainingConstructor()
  {
    var engine = EngineBuilder.ForInputAndOutput<string, string>()
                              .Build();
    Assert.Throws<ArgumentNullException>(
      () => new ChainedEngine<string, string, string>(null, engine, () => string.Empty));
    Assert.Throws<ArgumentNullException>(
      () => new ChainedEngine<string, string, string>(engine, null, () => string.Empty));
    Assert.Throws<ArgumentNullException>(
      () => new ChainedEngine<string, string, string>(engine, engine, null));
  }

  [Fact]
  public void ChainingConstructorAsync()
  {
    var engine = EngineBuilder.ForInputAndOutputAsync<string, string>()
                              .Build();
    Assert.Throws<ArgumentNullException>(
      () => new Rubric.Engines.Async.Implementation.ChainedEngine<string, string, string>(
        null, engine, () => string.Empty));
    Assert.Throws<ArgumentNullException>(
      () => new Rubric.Engines.Async.Implementation.ChainedEngine<string, string, string>(
        engine, null, () => string.Empty));
    Assert.Throws<ArgumentNullException>(
      () => new Rubric.Engines.Async.Implementation.ChainedEngine<string, string, string>(engine, engine, null));
  }

  [Fact]
  public async Task AsyncEngineChaining()
  {
    var engine = EngineBuilder.ForInputAndOutputAsync<TestInput, List<string>>()
                              .WithRule("first")
                              .WithAction((_, _, s) =>
                              {
                                s.Add("test");
                                return Task.CompletedTask;
                              })
                              .EndRule()
                              .Build();
    var engine2 = EngineBuilder.ForInputAndOutputAsync<List<string>, TestOutput>()
                               .WithRule("first")
                               .WithAction((_, i, o) =>
                               {
                                 foreach (var s in i)
                                   o.Outputs.Add(s);
                                 return Task.CompletedTask;
                               })
                               .EndRule()
                               .Build();
    var chained = engine.Chain(() => new(), engine2);
    Assert.IsType<Rubric.Engines.Async.Implementation.ChainedEngine<TestInput, List<string>, TestOutput>>(chained);
    var typed = (Rubric.Engines.Async.Implementation.ChainedEngine<TestInput, List<string>, TestOutput>)chained;
    Assert.Equal(engine, typed.First);
    Assert.Equal(engine2, typed.Second);
    Assert.Throws<NotImplementedException>(() => chained.Logger);
    Assert.Throws<NotImplementedException>(() => chained.ExceptionHandler);
    Assert.Equal(typeof(TestInput), chained.InputType);
    Assert.Equal(typeof(TestOutput), chained.OutputType);
    Assert.Equal(engine.PreRules, chained.PreRules);
    Assert.Equal(engine2.PostRules, chained.PostRules);
    Assert.Throws<NotImplementedException>(() => chained.Rules);
    Assert.True(chained.IsAsync);
    Assert.False(chained.IsParallel);
    var output = new TestOutput();
    var input = new TestInput();
    await chained.ApplyAsync(input, output);
    Assert.Contains("test", output.Outputs);
    output.Outputs.Clear();
    var inputs = new[] { input, new TestInput() };
    await chained.ApplyAsync(inputs, output);
    Assert.Contains("test", output.Outputs);
    Assert.Equal(2, output.Outputs.Count);
    output.Outputs.Clear();
    await chained.ApplyParallelAsync(inputs, output);
    Assert.Contains("test", output.Outputs);
    Assert.Equal(2, output.Outputs.Count);
    output.Outputs.Clear();
    await chained.ApplyAsync(inputs.ToAsyncEnumerable(), output);
    Assert.Contains("test", output.Outputs);
    Assert.Equal(2, output.Outputs.Count);
  }

  [Fact]
  public void AsyncEngineChainingParallel()
  {
    var engine = EngineBuilder.ForInputAndOutputAsync<TestInput, List<string>>()
                              .AsParallel()
                              .Build();
    var engine2 = EngineBuilder.ForInputAndOutputAsync<List<string>, TestOutput>()
                               .Build();
    var chained = engine.Chain(() => new(), engine2);
    Assert.True(chained.IsParallel);
  }

  [Fact]
  public void AsyncEngineChainingParallel2()
  {
    var engine = EngineBuilder.ForInputAndOutputAsync<TestInput, List<string>>()
                              .Build();
    var engine2 = EngineBuilder.ForInputAndOutputAsync<List<string>, TestOutput>()
                               .AsParallel()
                               .Build();
    var chained = engine.Chain(() => new(), engine2);
    Assert.True(chained.IsParallel);
  }
}
