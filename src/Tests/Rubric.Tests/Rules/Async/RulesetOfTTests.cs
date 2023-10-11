using Rubric.Rulesets.Async;
using Rubric.Tests.TestRules.Async;

namespace Rubric.Tests.Rules.Async;

public class RulesetOfTTests
{
  [Fact]
  public void AddAsyncRule()
  {
    var ruleset = new Ruleset<TestInput>();
    var rule = new TestDefaultAsyncPreRule();
    ruleset.AddAsyncRule(rule);
    Assert.Contains(rule, ruleset.Rules);
  }

  [Fact]
  public void AddAsyncRules()
  {
    var ruleset = new Ruleset<TestInput>();
    var rule = new TestDefaultAsyncPreRule();
    var rule2 = new TestDefaultAsyncPreRule();
    ruleset.AddAsyncRules(new[] { rule, rule2 });
    Assert.Contains(rule, ruleset.Rules);
    Assert.Contains(rule2, ruleset.Rules);
  }

  [Fact]
  public void Exceptions()
  {
    Assert.Throws<ArgumentNullException>(() => new Ruleset<TestInput>().AddAsyncRule(null));
    Assert.Throws<ArgumentNullException>(() => new Ruleset<TestInput>().AddAsyncRules(null));
  }
}
