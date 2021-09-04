using Rubric.Tests.TestRules.Async;

namespace Rubric.Tests.Rules.Async;

public class AyncRulesetTests
{
  [Fact]
  public void AddAsyncPostRule()
  {
    var ruleset = new AsyncRuleset<TestInput, TestOutput>();
    var postRule = new TestDefaultAsyncPostRule();
    ruleset.AddAsyncPostRule(postRule);
    Assert.Contains(postRule, ruleset.AsyncPostRules);
  }

  [Fact]
  public void AddAsyncPostRules()
  {
    var ruleset = new AsyncRuleset<TestInput, TestOutput>();
    var postRule = new TestDefaultAsyncPostRule();
    var postRule2 = new TestDefaultAsyncPostRule();
    ruleset.AddAsyncPostRules(new[] { postRule, postRule2 });
    Assert.Contains(postRule, ruleset.AsyncPostRules);
    Assert.Contains(postRule2, ruleset.AsyncPostRules);
  }

  [Fact]
  public void AddAsyncPreRule()
  {
    var ruleset = new AsyncRuleset<TestInput, TestOutput>();
    var preRule = new TestDefaultAsyncPreRule();
    ruleset.AddAsyncPreRule(preRule);
    Assert.Contains(preRule, ruleset.AsyncPreRules);
  }

  [Fact]
  public void AddAsyncPreRules()
  {
    var ruleset = new AsyncRuleset<TestInput, TestOutput>();
    var preRule = new TestDefaultAsyncPreRule();
    var preRule2 = new TestDefaultAsyncPreRule();
    ruleset.AddAsyncPreRules(new[] { preRule, preRule2 });
    Assert.Contains(preRule, ruleset.AsyncPreRules);
    Assert.Contains(preRule2, ruleset.AsyncPreRules);
  }

  [Fact]
  public void AddAsyncRule()
  {
    var ruleset = new AsyncRuleset<TestInput, TestOutput>();
    var rule = new TestDefaultAsyncRule();
    ruleset.AddAsyncRule(rule);
    Assert.Contains(rule, ruleset.AsyncRules);
  }

  [Fact]
  public void AddAsyncRules()
  {
    var ruleset = new AsyncRuleset<TestInput, TestOutput>();
    var rule = new TestDefaultAsyncRule();
    var rule2 = new TestDefaultAsyncRule();
    ruleset.AddAsyncRules(new[] { rule, rule2 });
    Assert.Contains(rule, ruleset.AsyncRules);
    Assert.Contains(rule2, ruleset.AsyncRules);
  }

  [Fact]
  public void Exceptions()
  {
    Assert.Throws<ArgumentNullException>(() => new AsyncRuleset<TestInput, TestOutput>().AddAsyncPreRule(null));
    Assert.Throws<ArgumentNullException>(() => new AsyncRuleset<TestInput, TestOutput>().AddAsyncRule(null));
    Assert.Throws<ArgumentNullException>(
        () => new AsyncRuleset<TestInput, TestOutput>().AddAsyncPostRule(null));
    Assert.Throws<ArgumentNullException>(
        () => new AsyncRuleset<TestInput, TestOutput>().AddAsyncPreRules(null));
    Assert.Throws<ArgumentNullException>(() => new AsyncRuleset<TestInput, TestOutput>().AddAsyncRules(null));
    Assert.Throws<ArgumentNullException>(
        () => new AsyncRuleset<TestInput, TestOutput>().AddAsyncPostRules(null));
  }
}
