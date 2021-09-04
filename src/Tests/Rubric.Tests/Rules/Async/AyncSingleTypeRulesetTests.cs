using Rubric.Tests.TestRules.Async;

namespace Rubric.Tests;

public class AyncSingleTypeRulesetTests
{
  [Fact]
  public void AddAsyncRule()
  {
    var ruleset = new AsyncRuleset<TestInput>();
    var rule = new TestDefaultAsyncPreRule();
    ruleset.AddAsyncRule(rule);
    Assert.Contains(rule, ruleset.AsyncRules);
  }

  [Fact]
  public void AddAsyncRules()
  {
    var ruleset = new AsyncRuleset<TestInput>();
    var rule = new TestDefaultAsyncPreRule();
    var rule2 = new TestDefaultAsyncPreRule();
    ruleset.AddAsyncRules(new[] { rule, rule2 });
    Assert.Contains(rule, ruleset.AsyncRules);
    Assert.Contains(rule2, ruleset.AsyncRules);
  }

  [Fact]
  public void Exceptions()
  {
    Assert.Throws<ArgumentNullException>(() => new AsyncRuleset<TestInput>().AddAsyncRule(null));
    Assert.Throws<ArgumentNullException>(() => new AsyncRuleset<TestInput>().AddAsyncRules(null));
  }
}
