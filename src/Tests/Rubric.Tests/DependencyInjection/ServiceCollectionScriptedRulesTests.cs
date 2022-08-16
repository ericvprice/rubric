using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace Rubric.Tests.DependencyInjection;

public class ServiceCollectionScriptedRulesTests
{
  [Fact]
  public void AddScriptedRulesOfT()
  {
    var services = new ServiceCollection();
    var configBuilder = new ConfigurationBuilder();
    var root = Directory.GetCurrentDirectory();
    var config = configBuilder.AddInMemoryCollection(new[]
                                {
                                    new KeyValuePair<string, string>(HostDefaults.ContentRootKey, root)
                                })
                              .SetBasePath(Directory.GetCurrentDirectory())
                              .AddJsonFile("Data\\appsettings.json")
                              .Build();
    services.AddAsyncRuleEngine<TestInput>()
            .AddScriptedRules<TestInput>(config, "ofT");
    var provider = services.BuildServiceProvider();
    var result = provider.GetService<IAsyncRuleEngine<TestInput>>();
    Assert.NotNull(result);
    Assert.Equal(2, result.Rules.Count());
    Assert.True(result.IsAsync);
  }

  [Fact]
  public void AddScriptedRulesOfTU()
  {
    var services = new ServiceCollection();
    var configBuilder = new ConfigurationBuilder();
    var root = Directory.GetCurrentDirectory();
    var config = configBuilder.AddInMemoryCollection(new[]
                                {
                                    new KeyValuePair<string, string>(HostDefaults.ContentRootKey, root)
                                })
                              .AddJsonFile("Data\\appsettings.json")
                              .Build();
    services.AddAsyncRuleEngine<TestInput, TestOutput>()
            .AddScriptedRules<TestInput, TestOutput>(config, "ofTU");
    var provider = services.BuildServiceProvider();
    var result = provider.GetService<IAsyncRuleEngine<TestInput, TestOutput>>();
    Assert.NotNull(result);
    Assert.Single(result.PreRules);
    Assert.Single(result.Rules);
    Assert.Single(result.PostRules);
    Assert.True(result.IsAsync);
  }
}
