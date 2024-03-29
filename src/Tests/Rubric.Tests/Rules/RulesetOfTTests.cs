using Rubric.Rulesets;
using Rubric.Tests.TestRules;

namespace Rubric.Tests.Rules;

public class RulesetOfTTests
{

  [Fact]
  public void AddRule()
  {
    var ruleset = new Ruleset<TestInput>();
    var rule = new TestPreRule(true);
    ruleset.AddRule(rule);
    Assert.Contains(rule, ruleset.Rules);
  }

  [Fact]
  public void AddRules()
  {
    var ruleset = new Ruleset<TestInput>();
    var rule = new TestPreRule(true);
    var rule2 = new TestPreRule(true);
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
