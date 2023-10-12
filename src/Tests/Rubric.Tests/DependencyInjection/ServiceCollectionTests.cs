﻿using Microsoft.Extensions.DependencyInjection;
using Rubric.Engines;
using Rubric.Tests.TestAssembly;
using Rubric.Tests.TestAssembly2;

namespace Rubric.Tests.DependencyInjection;

public class ServiceCollectionTests
{
  [Fact]
  public void LoadSyncFromAssemblyT()
  {
    var services = new ServiceCollection();
    services.AddRuleEngine<TestAssemblyInput>()
            .AddRules<TestAssemblyInput>();
    var provider = services.BuildServiceProvider();
    var result = provider.GetService<IRuleEngine<TestAssemblyInput>>();
    Assert.NotNull(result);
    Assert.Single(result.Rules);
    Assert.False(result.IsAsync);
  }

  [Fact]
  public void LoadSyncFromAssemblyTWithBuilder()
  {
    var services = new ServiceCollection();
    services.AddRuleEngine<TestAssemblyInput>(b => b.WithRule("foo")
                                                    .WithAction((_, _) => { })
                                                    .EndRule())
            .AddRules<TestAssemblyInput>();
    var provider = services.BuildServiceProvider();
    var result = provider.GetService<IRuleEngine<TestAssemblyInput>>();
    Assert.NotNull(result);
    Assert.Equal(2, result.Rules.Count());
    Assert.False(result.IsAsync);
  }

  [Fact]

  public void LoadSyncFromAssemblyTInTOut()
  {
    var services = new ServiceCollection();
    services.AddRuleEngine<TestAssemblyInput, TestAssemblyOutput>()
            .AddRules<TestAssemblyInput, TestAssemblyOutput>();
    var provider = services.BuildServiceProvider();
    var result = provider.GetService<IRuleEngine<TestAssemblyInput, TestAssemblyOutput>>();
    Assert.NotNull(result);
    Assert.Single(result.PreRules);
    Assert.Single(result.Rules);
    Assert.Single(result.PostRules);
    Assert.False(result.IsAsync);
  }

  [Fact]
  public void LoadSyncFromAssemblyTInTOutWithBuilder()
  {
    var services = new ServiceCollection();
    services.AddRuleEngine<TestAssemblyInput, TestAssemblyOutput>(
              b => b.WithRule("foo").WithAction((_, _, _) => { }).EndRule()
            )
            .AddRules<TestAssemblyInput, TestAssemblyOutput>();
    var provider = services.BuildServiceProvider();
    var result = provider.GetService<IRuleEngine<TestAssemblyInput, TestAssemblyOutput>>();
    Assert.NotNull(result);
    Assert.Single(result.PreRules);
    Assert.Equal(2, result.Rules.Count());
    Assert.Single(result.PostRules);
    Assert.False(result.IsAsync);
  }

  [Fact]

  public void LoadSyncFromAssemblyTInTOutIncludes()
  {
    var services = new ServiceCollection();
    services.AddRuleEngine<TestAssemblyInput, TestAssemblyOutput>()
            .AddRules<TestAssemblyInput, TestAssemblyOutput>(
              includes: new[]
              {
                "TestInputOutputRule"
              }
            );
    var provider = services.BuildServiceProvider();
    var result = provider.GetService<IRuleEngine<TestAssemblyInput, TestAssemblyOutput>>();
    Assert.NotNull(result);
    Assert.Empty(result.PreRules);
    Assert.Single(result.Rules);
    Assert.Empty(result.PostRules);
    Assert.False(result.IsAsync);
  }

  [Fact]

  public void LoadSyncFromAssemblyTInTOutExcludes()
  {
    var services = new ServiceCollection();
    services.AddRuleEngine<TestAssemblyInput, TestAssemblyOutput>()
            .AddRules<TestAssemblyInput, TestAssemblyOutput>(
              excludes: new[]
              {
                "TestInputOutputRule"
              }
            );
    var provider = services.BuildServiceProvider();
    var result = provider.GetService<IRuleEngine<TestAssemblyInput, TestAssemblyOutput>>();
    Assert.NotNull(result);
    Assert.Single(result.PreRules);
    Assert.Empty(result.Rules);
    Assert.Single(result.PostRules);
    Assert.False(result.IsAsync);
  }

  [Fact]
  public void LoadSyncFromAssemblyTInTOutCrossAssembly()
  {
    var services = new ServiceCollection();
    services.AddRuleEngine<TestAssemblyInput, TestAssemblyOutput2>()
            .AddRules<TestAssemblyInput, TestAssemblyOutput2>();
    var provider = services.BuildServiceProvider();
    var result = provider.GetService<IRuleEngine<TestAssemblyInput, TestAssemblyOutput2>>();
    Assert.NotNull(result);
    Assert.Equal(2, result.PreRules.Count());
    Assert.Single(result.Rules);
    Assert.Single(result.PostRules);
    Assert.False(result.IsAsync);
  }
}
