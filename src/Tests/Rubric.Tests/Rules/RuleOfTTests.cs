﻿using Rubric.Tests.TestRules;

namespace Rubric.Tests.Rules;

public class RuleOfTTests
{
  [Theory]
  [InlineData(true)]
  [InlineData(false)]
  public void DoesApply(bool expected)
  {
    var rule = new TestPreRule(expected);
    Assert.Equal(expected, rule.DoesApply(null, null));
  }

  [Theory]
  [InlineData(true)]
  [InlineData(false)]
  public void LambdaDoesApply(bool expected)
  {
    var rule = new LambdaRule<TestInput>("test", (_, _) => expected, (_, _) => { });
    Assert.Equal(expected, rule.DoesApply(null, null));
  }

  [Fact]
  public void DefaultDoesApply()
  {
    var rule = new TestDefaultPreRule();
    Assert.True(rule.DoesApply(null, null));
  }

  [Fact]
  public void Dependencies()
  {
    var rule = new DepTestPreRule(true);
    var dependencies = rule.Dependencies.ToList();
    Assert.Contains("dep1", dependencies);
    Assert.Contains("dep2", dependencies);
    Assert.Equal(2, dependencies.Count);
  }

  [Fact]
  public void LambdaConstructor()
  {
    var rule = new LambdaRule<TestInput>("test", (_, _) => true, (_, _) => { }, new[] { "dep1", "dep2" },
                                            new[] { "prv1", "prv2" });
    Assert.Equal("test", rule.Name);
    Assert.Contains("dep1", rule.Dependencies);
    Assert.Contains("dep2", rule.Dependencies);
    Assert.Contains("prv1", rule.Provides);
    Assert.Contains("prv2", rule.Provides);
  }

  [Fact]
  public void LambdaConstructorException()
  {
    Assert.Throws<ArgumentNullException>(() => new LambdaRule<TestInput>("test", null, (_, _) => { }));
    Assert.Throws<ArgumentNullException>(() => new LambdaRule<TestInput>("test", (_, _) => true, null));
    Assert.Throws<ArgumentException>(
        () => new LambdaRule<TestInput>(null, (_, _) => true, (_, _) => { }));
  }

  [Fact]
  public void Provides()
  {
    var rule = new DepTestPreRule(true);
    var provides = rule.Provides.ToList();
    Assert.Contains("dep3", provides);
    Assert.Contains(typeof(DepTestPreRule).FullName, provides);
    Assert.Equal(3, provides.Count);
  }
}
