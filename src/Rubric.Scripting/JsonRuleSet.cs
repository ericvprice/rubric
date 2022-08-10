using System;
using Rubric.Rules.Async;
using static Rubric.Scripting.RuleLoader;

namespace Rubric.Scripting
{

  public class JsonRuleSet<T> : AsyncRuleset<T>
  {
    public JsonRuleSet(string ruleGlob) : base()
    {
      if (string.IsNullOrWhiteSpace(ruleGlob))
      {
        throw new ArgumentException($"'{nameof(ruleGlob)}' cannot be null or whitespace.", nameof(ruleGlob));
      }
      AddAsyncRules(LoadJsonFromGlob<T>(ruleGlob));
    }
  }

  public class JsonRuleSet<T, U> : AsyncRuleset<T, U>
  {
    public JsonRuleSet(string preRuleGlob, string ruleGlob, string postRuleGlob) : base()
    {
      if (string.IsNullOrWhiteSpace(preRuleGlob))
        throw new ArgumentException($"'{nameof(preRuleGlob)}' cannot be null or whitespace.", nameof(preRuleGlob));
      if (string.IsNullOrWhiteSpace(ruleGlob))
        throw new ArgumentException($"'{nameof(ruleGlob)}' cannot be null or whitespace.", nameof(ruleGlob));
      if (string.IsNullOrWhiteSpace(postRuleGlob))
        throw new ArgumentException($"'{nameof(postRuleGlob)}' cannot be null or whitespace.", nameof(postRuleGlob));
      AddAsyncPreRules(LoadJsonFromGlob<T>(preRuleGlob));
      AddAsyncRules(LoadJsonFromGlob<T, U>(ruleGlob));
      AddAsyncPostRules(LoadJsonFromGlob<U>(postRuleGlob));
    }
  }
}