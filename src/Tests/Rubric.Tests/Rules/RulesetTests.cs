using Rubric.Tests.TestRules;

namespace Rubric.Tests.Rules;

public class RulesetTests
{
  [Fact]
  public void AddPostRule()
  {
    var ruleset = new Ruleset<TestInput, TestOutput>();
    var postRule = new TestDefaultPostRule();
    ruleset.AddPostRule(postRule);
    Assert.Contains(postRule, ruleset.PostRules);
  }

  [Fact]
  public void AddPostRules()
  {
    var ruleset = new Ruleset<TestInput, TestOutput>();
    var postRule = new TestDefaultPostRule();
    var postRule2 = new TestDefaultPostRule();
    ruleset.AddPostRules(new[] { postRule, postRule2 });
    Assert.Contains(postRule, ruleset.PostRules);
    Assert.Contains(postRule2, ruleset.PostRules);
  }

  [Fact]
  public void AddPreRule()
  {
    var ruleset = new Ruleset<TestInput, TestOutput>();
    var preRule = new TestDefaultPreRule();
    ruleset.AddPreRule(preRule);
    Assert.Contains(preRule, ruleset.PreRules);
  }

  [Fact]
  public void AddPreRules()
  {
    var ruleset = new Ruleset<TestInput, TestOutput>();
    var preRule = new TestDefaultPreRule();
    var preRule2 = new TestDefaultPreRule();
    ruleset.AddPreRules(new[] { preRule, preRule2 });
    Assert.Contains(preRule, ruleset.PreRules);
    Assert.Contains(preRule2, ruleset.PreRules);
  }

  [Fact]
  public void AddRule()
  {
    var ruleset = new Ruleset<TestInput, TestOutput>();
    var rule = new TestDefaultRule();
    ruleset.AddRule(rule);
    Assert.Contains(rule, ruleset.Rules);
  }

  [Fact]
  public void AddRules()
  {
    var ruleset = new Ruleset<TestInput, TestOutput>();
    var rule = new TestDefaultRule();
    var rule2 = new TestDefaultRule();
    ruleset.AddRules(new[] { rule, rule2 });
    Assert.Contains(rule, ruleset.Rules);
    Assert.Contains(rule2, ruleset.Rules);
  }

  [Fact]
  public void Exceptions()
  {
    Assert.Throws<ArgumentNullException>(() => new Ruleset<TestInput, TestOutput>().AddPreRule(null));
    Assert.Throws<ArgumentNullException>(() => new Ruleset<TestInput, TestOutput>().AddRule(null));
    Assert.Throws<ArgumentNullException>(() => new Ruleset<TestInput, TestOutput>().AddPostRule(null));
    Assert.Throws<ArgumentNullException>(() => new Ruleset<TestInput, TestOutput>().AddPreRules(null));
    Assert.Throws<ArgumentNullException>(() => new Ruleset<TestInput, TestOutput>().AddRules(null));
    Assert.Throws<ArgumentNullException>(() => new Ruleset<TestInput, TestOutput>().AddPostRules(null));
  }
}
