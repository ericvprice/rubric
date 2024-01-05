using Rubric.Builder;
using Rubric.Engines.Implementation;
using Rubric.Tests.TestRules;

namespace Rubric.Tests.Builders;

public class BuilderOfTInTOutTests
{

  [Fact]
  public void EmptyBuilder()
  {
    var builder = EngineBuilder.ForInputAndOutput<TestInput, TestOutput>();
    Assert.NotNull(builder.ExceptionHandler);
    Assert.NotNull(builder.Logger);
  }

  [Fact]
  public void EmptyEngineConstruction()
  {
    var engine = EngineBuilder.ForInputAndOutput<TestInput, TestOutput>()
                              .WithExceptionHandler(ExceptionHandlers.Ignore)
                              .Build();
    Assert.NotNull(engine);
    Assert.Equal(ExceptionHandlers.Ignore, engine.ExceptionHandler);
    var input = new TestInput();
    var output = new TestOutput();
    //Just assert nothing goes wrong with a blank engine
    engine.Apply(input, output);
  }

  [Fact]
  public void EmptyEngineConstructionWithLogger()
  {
    var logger = new TestLogger();
    var engine = EngineBuilder.ForInputAndOutput<TestInput, TestOutput>(logger)
                              .Build();
    Assert.NotNull(engine);
    Assert.Equal(logger, engine.Logger);
    var input = new TestInput();
    var output = new TestOutput();
    //Just assert nothing goes wrong with a blank engine
    engine.Apply(input, output);
  }

  [Fact]
  public void EngineConstruction()
  {
    var engine = EngineBuilder.ForInputAndOutput<TestInput, TestOutput>()
                              .WithPreRule(new TestDefaultPreRule())
                              .WithRule(new TestDefaultRule())
                              .WithPostRule(new TestDefaultPostRule())
                              .Build();
    Assert.NotNull(engine);
    Assert.Single(engine.PreRules);
    Assert.Single(engine.Rules);
    Assert.Single(engine.PostRules);
  }

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
                               .WithAction((_, i, o) => o.Outputs.Add(i.First()))
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
  }

  [Fact]
  public void LambdaPostRuleConstruction()
  {
    var engine = EngineBuilder.ForInputAndOutput<TestInput, TestOutput>()
                              .WithPostRule(new TestPostRule(true))
                              .WithPostRule("test")
                              .WithPredicate((_, _) => true)
                              .WithAction((_, _) => { })
                              .ThatProvides("test1")
                              .EndRule()
                              .WithPostRule("test2")
                              .WithPredicate((_, _) => true)
                              .WithAction((_, _) => { })
                              .ThatDependsOn("test")
                              .ThatDependsOn(typeof(TestPostRule))
                              .EndRule()
                              .Build();
    Assert.Equal(3, engine.PostRules.Count());
    var rule = engine.PostRules.ElementAt(1);
    Assert.Equal("test", rule.Name);
    Assert.Contains("test", rule.Provides);
    Assert.Contains("test1", rule.Provides);
    rule = engine.PostRules.ElementAt(2);
    Assert.Contains("test", rule.Dependencies);
    Assert.Contains(typeof(TestPostRule).FullName, rule.Dependencies);
    Assert.True(rule.DoesApply(null, null));
  }

  [Fact]
  public void EngineCopying()
  {
    var engine = EngineBuilder.ForInputAndOutput<TestInput, TestOutput>()
                              .WithPostRule(new TestPostRule(true))
                              .WithPostRule("test")
                              .WithPredicate((_, _) => true)
                              .WithAction((_, _) => { })
                              .ThatProvides("test1")
                              .EndRule()
                              .WithPostRule("test2")
                              .WithPredicate((_, _) => true)
                              .WithAction((_, _) => { })
                              .ThatDependsOn("test")
                              .ThatDependsOn(typeof(TestPostRule))
                              .EndRule()
                              .Build();
    var engine2 = EngineBuilder.FromEngine(engine).Build();
    Assert.Equal(3, engine2.PostRules.Count());
    var rule = engine2.PostRules.ElementAt(1);
    Assert.Equal("test", rule.Name);
    Assert.Contains("test", rule.Provides);
    Assert.Contains("test1", rule.Provides);
    rule = engine2.PostRules.ElementAt(2);
    Assert.Contains("test", rule.Dependencies);
    Assert.Contains(typeof(TestPostRule).FullName, rule.Dependencies);
    Assert.True(rule.DoesApply(null, null));
  }


  [Fact]
  public void LambdaPostRuleConstructionThrowsOnNullOrEmpty()
  {
    Assert.Throws<ArgumentException>(
        () =>
            EngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>()
                .WithPostRule((string)null)
    );
    Assert.Throws<ArgumentException>(
        () =>
            EngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithPostRule("")
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithPostRule("foo")
                .WithAction(null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithPostRule("foo")
                .WithPredicate(null)
    );
    Assert.Throws<ArgumentException>(
        () =>
            EngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithPostRule("foo")
                .ThatProvides(null)
    );
    Assert.Throws<ArgumentException>(
        () =>
            EngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithPostRule("foo")
                .ThatProvides("")
    );
    Assert.Throws<ArgumentException>(
        () =>
            EngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithPostRule("foo")
                .ThatDependsOn("")
    );
    Assert.Throws<ArgumentException>(
        () =>
            EngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithPostRule("foo")
                .ThatDependsOn((string)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithPostRule("foo")
                .ThatDependsOn((Type)null)
    );
  }

  [Fact]
  public void LambdaPreRuleConstruction()
  {
    var engine = EngineBuilder.ForInputAndOutput<TestInput, TestOutput>()
                              .WithPreRule(new TestPreRule(true))
                              .WithPreRule("test")
                              .WithPredicate((_, _) => true)
                              .WithAction((_, _) => { })
                              .ThatProvides("test1")
                              .EndRule()
                              .WithPreRule("test2")
                              .WithPredicate((_, _) => true)
                              .WithAction((_, _) => { })
                              .ThatDependsOn("test")
                              .ThatDependsOn(typeof(TestPreRule))
                              .EndRule()
                              .Build();
    Assert.Equal(3, engine.PreRules.Count());
    var rule = engine.PreRules.ElementAt(1);
    Assert.Equal("test", rule.Name);
    Assert.Contains("test1", rule.Provides);
    rule = engine.PreRules.ElementAt(2);
    Assert.Contains(typeof(TestPreRule).FullName, rule.Dependencies);
    Assert.Contains("test", rule.Dependencies);
    Assert.True(rule.DoesApply(null, null));
  }

  [Fact]
  public void LambdaPreRuleConstructionThrowsOnNullOrEmpty()
  {
    Assert.Throws<ArgumentException>(
        () =>
            EngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>()
                .WithPreRule((string)null)
    );
    Assert.Throws<ArgumentException>(
        () =>
            EngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithPreRule("")
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithPreRule("foo")
                .WithAction(null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithPreRule("foo")
                .WithPredicate(null)
    );
    Assert.Throws<ArgumentException>(
        () =>
            EngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithPreRule("foo")
                .ThatProvides(null)
    );
    Assert.Throws<ArgumentException>(
        () =>
            EngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithPreRule("foo")
                .ThatProvides("")
    );
    Assert.Throws<ArgumentException>(
        () =>
            EngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithPreRule("foo")
                .ThatDependsOn("")
    );
    Assert.Throws<ArgumentException>(
        () =>
            EngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithPreRule("foo")
                .ThatDependsOn((string)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithPreRule("foo")
                .ThatDependsOn((Type)null)
    );
  }

  [Fact]
  public void LambdaRuleConstruction()
  {
    var engine = EngineBuilder.ForInputAndOutput<TestInput, TestOutput>()
                              .WithRule(new TestRule(true))
                              .WithRule("test")
                              .WithPredicate((_, _, _) => true)
                              .WithAction((_, _, _) => { })
                              .ThatProvides("test1")
                              .EndRule()
                              .WithRule("test2")
                              .WithPredicate((_, _, _) => true)
                              .WithAction((_, _, _) => { })
                              .ThatDependsOn(typeof(TestRule))
                              .ThatDependsOn("test1")
                              .EndRule()
                              .Build();
    Assert.Equal(3, engine.Rules.Count());
    var rule = engine.Rules.ElementAt(1);
    Assert.Equal("test", rule.Name);
    Assert.Contains("test1", rule.Provides);
    rule = engine.Rules.ElementAt(2);
    Assert.Contains(typeof(TestRule).FullName, rule.Dependencies);
    Assert.Contains("test1", rule.Dependencies);
    Assert.True(rule.DoesApply(null, null, null));
  }


  [Fact]
  public void LambdaRuleConstructionThrowsOnNullOrEmpty()
  {
    Assert.Throws<ArgumentException>(
        () =>
            EngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithRule((string)null)
    );
    Assert.Throws<ArgumentException>(
        () =>
            EngineBuilder.ForInputAndOutput<TestInput, TestOutput>().WithRule("")
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithRule("foo")
                .WithAction(null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithRule("foo")
                .WithPredicate(null)
    );
    Assert.Throws<ArgumentException>(
        () =>
            EngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithRule("foo")
                .ThatProvides(null)
    );
    Assert.Throws<ArgumentException>(
        () =>
            EngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithRule("foo")
                .ThatProvides("")
    );
    Assert.Throws<ArgumentException>(
        () =>
            EngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithRule("foo")
                .ThatDependsOn("")
    );
    Assert.Throws<ArgumentException>(
        () =>
            EngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithRule("foo")
                .ThatDependsOn((string)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            EngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithRule("foo")
                .ThatDependsOn((Type)null)
    );
  }

  [Fact]
  public void TypeAttributeDependency()
  {
    var engine = EngineBuilder.ForInputAndOutput<TestInput, TestOutput>()
                              .WithPreRules(new [] { new DepTestAttrPreRule(true), new DepTestAttrPreRule2(true) })
                              .WithRules(new [] { new DepTestAttrRule(true), new DepTestAttrRule2(true)})
                              .WithPostRules(new [] { new DepTestAttrPostRule(true), new DepTestAttrPostRule2(true) })
                              .Build();
    Assert.NotNull(engine);
  }
}
