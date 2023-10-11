using Rubric.Rulesets.Async;
using Rubric.Tests.TestRules.Async;

namespace Rubric.Tests.Rules.Async;

public class RulesetOfTInTOutTests
{

  [Fact]
  public void AddAsyncPostRule()
  {
    var ruleset = new Ruleset<TestInput, TestOutput>();
    var postRule = new TestDefaultAsyncPostRule();
    ruleset.AddAsyncPostRule(postRule);
    Assert.Contains(postRule, ruleset.PostRules);
  }

  [Fact]
  public void AddAsyncPostRules()
  {
    var ruleset = new Ruleset<TestInput, TestOutput>();
    var postRule = new TestDefaultAsyncPostRule();
    var postRule2 = new TestDefaultAsyncPostRule();
    ruleset.AddAsyncPostRules(new[] { postRule, postRule2 });
    Assert.Contains(postRule, ruleset.PostRules);
    Assert.Contains(postRule2, ruleset.PostRules);
  }

  [Fact]
  public void AddAsyncPreRule()
  {
    var ruleset = new Ruleset<TestInput, TestOutput>();
    var preRule = new TestDefaultAsyncPreRule();
    ruleset.AddAsyncPreRule(preRule);
    Assert.Contains(preRule, ruleset.PreRules);
  }

  [Fact]
  public void AddAsyncPreRules()
  {
    var ruleset = new Ruleset<TestInput, TestOutput>();
    var preRule = new TestDefaultAsyncPreRule();
    var preRule2 = new TestDefaultAsyncPreRule();
    ruleset.AddAsyncPreRules(new[] { preRule, preRule2 });
    Assert.Contains(preRule, ruleset.PreRules);
    Assert.Contains(preRule2, ruleset.PreRules);
  }

  [Fact]
  public void AddAsyncRule()
  {
    var ruleset = new Ruleset<TestInput, TestOutput>();
    var rule = new TestDefaultAsyncRule();
    ruleset.AddAsyncRule(rule);
    Assert.Contains(rule, ruleset.Rules);
  }

  [Fact]
  public void AddAsyncRules()
  {
    var ruleset = new Ruleset<TestInput, TestOutput>();
    var rule = new TestDefaultAsyncRule();
    var rule2 = new TestDefaultAsyncRule();
    ruleset.AddAsyncRules(new[] { rule, rule2 });
    Assert.Contains(rule, ruleset.Rules);
    Assert.Contains(rule2, ruleset.Rules);
  }

  [Fact]
  public void Exceptions()
  {
    Assert.Throws<ArgumentNullException>(() => new Ruleset<TestInput, TestOutput>().AddAsyncPreRule(null));
    Assert.Throws<ArgumentNullException>(() => new Ruleset<TestInput, TestOutput>().AddAsyncRule(null));
    Assert.Throws<ArgumentNullException>(
        () => new Ruleset<TestInput, TestOutput>().AddAsyncPostRule(null));
    Assert.Throws<ArgumentNullException>(
        () => new Ruleset<TestInput, TestOutput>().AddAsyncPreRules(null));
    Assert.Throws<ArgumentNullException>(() => new Ruleset<TestInput, TestOutput>().AddAsyncRules(null));
    Assert.Throws<ArgumentNullException>(
        () => new Ruleset<TestInput, TestOutput>().AddAsyncPostRules(null));
  }
}
