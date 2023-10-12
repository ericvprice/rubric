using Rubric.Rules.Probabilistic;
using Rubric.Tests.TestRules.Probabilistic;

namespace Rubric.Tests.Builders;

public class ProbabilisticBuilderOfTInTOutTests
{
  [Fact]
  public void EmptyBuilder()
  {
    var builder = ProbabilisticEngineBuilder.ForInputAndOutput<TestInput, TestOutput>();
    Assert.NotNull(builder.ExceptionHandler);
    Assert.NotNull(builder.Logger);
  }

  [Fact]
  public void EmptyEngineConstruction()
  {
    var engine = ProbabilisticEngineBuilder.ForInputAndOutput<TestInput, TestOutput>()
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
    var engine = ProbabilisticEngineBuilder.ForInputAndOutput<TestInput, TestOutput>(logger)
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
    var engine = ProbabilisticEngineBuilder.ForInputAndOutput<TestInput, TestOutput>()
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
  public void LambdaPostRuleConstruction()
  {
    var engine = ProbabilisticEngineBuilder.ForInputAndOutput<TestInput, TestOutput>()
                              .WithPostRule(new TestPostRule(1D))
                              .WithPostRule("test")
                              .WithPredicate((_, _) => 1D)
                              .WithAction((_, _) => { })
                              .ThatProvides("test1")
                              .EndRule()
                              .WithPostRule("test2")
                              .WithPredicate((_, _) => 1D)
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
    Assert.Equal(1D, rule.DoesApply(null, null));
  }


  [Fact]
  public void LambdaPostRuleConstructionThrowsOnNullOrEmpty()
  {
    Assert.Throws<ArgumentException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>()
                .WithPostRule((string)null)
    );
    Assert.Throws<ArgumentException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithPostRule("")
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithPostRule("foo")
                .WithAction(null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithPostRule("foo")
                .WithPredicate(null)
    );
    Assert.Throws<ArgumentException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithPostRule("foo")
                .ThatProvides(null)
    );
    Assert.Throws<ArgumentException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithPostRule("foo")
                .ThatProvides("")
    );
    Assert.Throws<ArgumentException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithPostRule("foo")
                .ThatDependsOn("")
    );
    Assert.Throws<ArgumentException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithPostRule("foo")
                .ThatDependsOn((string)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithPostRule("foo")
                .ThatDependsOn((Type)null)
    );
  }

  [Fact]
  public void LambdaPreRuleConstruction()
  {
    var engine = ProbabilisticEngineBuilder.ForInputAndOutput<TestInput, TestOutput>()
                              .WithPreRule(new TestPreRule(1D))
                              .WithPreRule("test")
                              .WithPredicate((_, _) => 1D)
                              .WithAction((_, _) => { })
                              .ThatProvides("test1")
                              .EndRule()
                              .WithPreRule("test2")
                              .WithPredicate((_, _) => 1D)
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
    Assert.Equal(1D, rule.DoesApply(null, null));
  }

  [Fact]
  public void LambdaPreRuleConstructionThrowsOnNullOrEmpty()
  {
    Assert.Throws<ArgumentException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>()
                .WithPreRule((string)null)
    );
    Assert.Throws<ArgumentException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithPreRule("")
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithPreRule("foo")
                .WithAction(null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithPreRule("foo")
                .WithPredicate(null)
    );
    Assert.Throws<ArgumentException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithPreRule("foo")
                .ThatProvides(null)
    );
    Assert.Throws<ArgumentException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithPreRule("foo")
                .ThatProvides("")
    );
    Assert.Throws<ArgumentException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithPreRule("foo")
                .ThatDependsOn("")
    );
    Assert.Throws<ArgumentException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithPreRule("foo")
                .ThatDependsOn((string)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithPreRule("foo")
                .ThatDependsOn((Type)null)
    );
  }

  [Fact]
  public void LambdaRuleConstruction()
  {
    var engine = ProbabilisticEngineBuilder.ForInputAndOutput<TestInput, TestOutput>()
                              .WithRule(new TestRule(1D))
                              .WithRule("test")
                              .WithPredicate((_, _, _) => 1D)
                              .WithAction((_, _, _) => { })
                              .ThatProvides("test1")
                              .EndRule()
                              .WithRule("test2")
                              .WithPredicate((_, _, _) => 1D)
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
    Assert.Equal(1D, rule.DoesApply(null, null, null));
  }


  [Fact]
  public void LambdaRuleConstructionThrowsOnNullOrEmpty()
  {
    Assert.Throws<ArgumentException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithRule((string)null)
    );
    Assert.Throws<ArgumentException>(
        () =>
            ProbabilisticEngineBuilder.ForInputAndOutput<TestInput, TestOutput>().WithRule("")
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithRule("foo")
                .WithAction(null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithRule("foo")
                .WithPredicate(null)
    );
    Assert.Throws<ArgumentException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithRule("foo")
                .ThatProvides(null)
    );
    Assert.Throws<ArgumentException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithRule("foo")
                .ThatProvides("")
    );
    Assert.Throws<ArgumentException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithRule("foo")
                .ThatDependsOn("")
    );
    Assert.Throws<ArgumentException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithRule("foo")
                .ThatDependsOn((string)null)
    );
    Assert.Throws<ArgumentNullException>(
        () =>
            ProbabilisticEngineBuilder
                .ForInputAndOutput<TestInput, TestOutput>().WithRule("foo")
                .ThatDependsOn((Type)null)
    );
  }

  [Fact]
  public void TypeAttributeDependency()
  {
    var engine = ProbabilisticEngineBuilder.ForInputAndOutput<TestInput, TestOutput>()
                              .WithPreRules(new Rule<TestInput>[] {new DepTestAttrPreRule(1D), new DepTestAttrPreRule2(1D)})
                              .WithRules(new Rule<TestInput, TestOutput> [] { new DepTestAttrRule(1D), new DepTestAttrRule2(1D)})
                              .WithPostRules(new Rule<TestOutput> [] { new DepTestAttrPostRule(1D), new DepTestAttrPostRule2(1D)})
                              .Build();
    Assert.NotNull(engine);
  }
}
