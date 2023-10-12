using Rubric.Tests.TestRules;

namespace Rubric.Tests.Builders;

public class BuilderOfTTests
{
  [Fact]
  public void EmptyEngineConstruction()
  {
    var engine = EngineBuilder.ForInput<TestInput>()
                              .Build();
    Assert.NotNull(engine);
    var input = new TestInput();
    //Just assert nothing goes wrong with a blank engine
    engine.Apply(input);
  }

  [Fact]
  public void EmptyEngineConstructionWithLogger()
  {
    var logger = new TestLogger();
    var engine = EngineBuilder.ForInput<TestInput>(logger)
                              .Build();
    Assert.NotNull(engine);
    Assert.Equal(logger, engine.Logger);
    var input = new TestInput();
    //Just assert nothing goes wrong with a blank engine
    engine.Apply(input);
  }

  [Fact]
  public void EngineConstruction()
  {
    var engine = EngineBuilder.ForInput<TestInput>()
                              .WithRule(new TestPreRule(true))
                              .WithExceptionHandler(ExceptionHandlers.Ignore)
                              .Build();
    Assert.NotNull(engine);
    Assert.Single(engine.Rules);
    Assert.Equal(ExceptionHandlers.Ignore, engine.ExceptionHandler);
  }

  [Fact]
  public void LambdaRuleConstruction()
  {
    var engine = EngineBuilder.ForInput<TestInput>()
                              .WithRule(new TestPreRule(true))
                              .WithRule("test")
                              .WithAction((_, _) => { })
                              .ThatProvides("test1")
                              .EndRule()
                              .WithRule("test2")
                              .WithPredicate((_, _) => true)
                              .WithAction((_, _) => { })
                              .ThatDependsOn(typeof(TestPreRule))
                              .ThatDependsOn("test1")
                              .EndRule()
                              .Build();
    Assert.Equal(3, engine.Rules.Count());
    var rule = engine.Rules.ElementAt(1);
    Assert.Equal("test", rule.Name);
    Assert.Contains("test1", rule.Provides);
    Assert.True(rule.DoesApply(null, null));
    rule = engine.Rules.ElementAt(2);
    Assert.Contains(typeof(TestPreRule).FullName, rule.Dependencies);
    Assert.Contains("test1", rule.Dependencies);
    Assert.True(rule.DoesApply(null, null));
  }


  [Fact]
  public void LambdaRuleConstructionThrowsOnNullOrEmpty()
  {
    Assert.Throws<ArgumentException>(() =>
                                         EngineBuilder
                                             .ForInput<TestInput>().WithRule((string)null)
    );
    Assert.Throws<ArgumentException>(() =>
                                         EngineBuilder.ForInput<TestInput>().WithRule("")
    );
    Assert.Throws<ArgumentNullException>(() =>
                                             EngineBuilder
                                                 .ForInput<TestInput>().WithRule("foo")
                                                 .WithAction(null)
    );
    Assert.Throws<ArgumentNullException>(() =>
                                             EngineBuilder
                                                 .ForInput<TestInput>().WithRule("foo")
                                                 .WithPredicate(null)
    );
    Assert.Throws<ArgumentException>(() =>
                                         EngineBuilder
                                             .ForInput<TestInput>().WithRule("foo")
                                             .ThatProvides(null)
    );
    Assert.Throws<ArgumentException>(() =>
                                         EngineBuilder
                                             .ForInput<TestInput>().WithRule("foo")
                                             .ThatProvides("")
    );
    Assert.Throws<ArgumentException>(() =>
                                         EngineBuilder
                                             .ForInput<TestInput>().WithRule("foo")
                                             .ThatDependsOn("")
    );
    Assert.Throws<ArgumentException>(() =>
                                         EngineBuilder
                                             .ForInput<TestInput>().WithRule("foo")
                                             .ThatDependsOn((string)null)
    );
    Assert.Throws<ArgumentNullException>(() =>
                                             EngineBuilder
                                                 .ForInput<TestInput>().WithRule("foo")
                                                 .ThatDependsOn((Type)null)
    );
  }

  [Fact]
  public void TypeAttributeDependency()
  {
    var engine = EngineBuilder.ForInput<TestInput>()
                              .WithRules(new [] {new DepTestAttrPreRule(true), new DepTestAttrPreRule2(true)})
                              .Build();
    Assert.NotNull(engine);
  }
}
