using System.Linq;
using Microsoft.Extensions.DependencyInjection;
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
  public void LoadAsyncFromAssemblyT()
  {
    var services = new ServiceCollection();
    services.AddAsyncRuleEngine<TestAssemblyInput>()
            .AddAsyncRules<TestAssemblyInput>();
    var provider = services.BuildServiceProvider();
    var result = provider.GetService<IAsyncRuleEngine<TestAssemblyInput>>();
    Assert.NotNull(result);
    Assert.Single(result.Rules);
    Assert.True(result.IsAsync);
  }

  [Fact]

  public void LoadAsyncFromAssemblyTInTOut()
  {
    var services = new ServiceCollection();
    services.AddAsyncRuleEngine<TestAssemblyInput, TestAssemblyOutput>()
            .AddAsyncRules<TestAssemblyInput, TestAssemblyOutput>();
    var provider = services.BuildServiceProvider();
    var result = provider.GetService<IAsyncRuleEngine<TestAssemblyInput, TestAssemblyOutput>>();
    Assert.NotNull(result);
    Assert.Single(result.PreRules);
    Assert.Single(result.Rules);
    Assert.Single(result.PostRules);
    Assert.True(result.IsAsync);
  }

  [Fact]

  public void LoadAsyncFromAssemblyTInTOutCrossAssembly()
  {
    var services = new ServiceCollection();
    services.AddAsyncRuleEngine<TestAssemblyInput, TestAssemblyOutput2>()
            .AddAsyncRules<TestAssemblyInput, TestAssemblyOutput2>();
    var provider = services.BuildServiceProvider();
    var result = provider.GetService<IAsyncRuleEngine<TestAssemblyInput, TestAssemblyOutput2>>();
    Assert.NotNull(result);
    Assert.Equal(2, result.PreRules.Count());
    Assert.Single(result.Rules);
    Assert.Single(result.PostRules);
    Assert.True(result.IsAsync);
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
