using Rubric.Rulesets.Probabilistic.Async;
using Rubric.Tests.TestRules.Probabilistic.Async;

namespace Rubric.Tests.Rules.Probabilistic.Async;

public class RulesetOfTTests
{
  [Fact]
  public void AddAsyncRule()
  {
    var ruleset = new Ruleset<TestInput>();
    var rule = new TestDefaultPreRule();
    ruleset.AddRule(rule);
    Assert.Contains(rule, ruleset.Rules);
  }

  [Fact]
  public void AddAsyncRules()
  {
    var ruleset = new Ruleset<TestInput>();
    var rule = new TestDefaultPreRule();
    var rule2 = new TestDefaultPreRule();
    ruleset.AddRules(new[] { rule, rule2 });
    Assert.Contains(rule, ruleset.Rules);
    Assert.Contains(rule2, ruleset.Rules);
  }

  [Fact]
  public void Exceptions()
  {
    Assert.Throws<ArgumentNullException>(() => new Ruleset<TestInput>().AddRule(null));
    Assert.Throws<ArgumentNullException>(() => new Ruleset<TestInput>().AddRules(null));
  }
}
