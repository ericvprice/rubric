using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using Rubric.Tests.TestAssembly3;
using Rubric.Engines.Async;
using Rubric.Extensions;

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
                              .AddJsonFile(Path.Combine("Data", "appsettings.json"))
                              .Build();
    services.AddAsyncRuleEngine<TestInput>()
            .AddScriptedRules<TestInput>(config, "ofT");
    var provider = services.BuildServiceProvider();
    var result = provider.GetService<IRuleEngine<TestInput>>();
    Assert.NotNull(result);
    Assert.Equal(2, result.Rules.Count());
    Assert.True(result.IsAsync);
  }

  [Fact]
  public void AddScriptedRulesOfTConfigOptions()
  {
    IServiceCollection services = new ServiceCollection();
    var configBuilder = new ConfigurationBuilder();
    var root = Directory.GetCurrentDirectory();
    var config = configBuilder.AddInMemoryCollection(new[]
                                {
                                    new KeyValuePair<string, string>(HostDefaults.ContentRootKey, root)
                                })
                              .SetBasePath(Directory.GetCurrentDirectory())
                              .AddJsonFile(Path.Combine("Data", "appsettings.json"))
                              .Build();
    services = services.AddAsyncRuleEngine<TestInput>()
                        .AddScriptedRules<TestInput>(
                          config,
                          "ofTWithDeps",
                          options => options.AddReferences(typeof(TestDep).Assembly));
    var provider = services.BuildServiceProvider();
    var result = provider.GetService<IRuleEngine<TestInput>>();
    Assert.NotNull(result);
    Assert.Equal(2, result.Rules.Count());
    Assert.True(result.IsAsync);
  }

  [Fact]
  public void AddScriptedRulesOfTInTOut()
  {
    IServiceCollection services = new ServiceCollection();
    var configBuilder = new ConfigurationBuilder();
    var root = Directory.GetCurrentDirectory();
    var config = configBuilder.AddInMemoryCollection(new[]
                                {
                                    new KeyValuePair<string, string>(HostDefaults.ContentRootKey, root)
                                })
                              .AddJsonFile(Path.Combine("Data", "appsettings.json"))
                              .Build();
    services = services.AddAsyncRuleEngine<TestInput, TestOutput>()
                       .AddScriptedRules<TestInput, TestOutput>(config, "ofTU");
    var provider = services.BuildServiceProvider();
    var result = provider.GetService<IRuleEngine<TestInput, TestOutput>>();
    Assert.NotNull(result);
    Assert.Single(result.PreRules);
    Assert.Single(result.Rules);
    Assert.Single(result.PostRules);
    Assert.True(result.IsAsync);
  }

  [Fact]
  public void AddScriptedRulesOfTInTOutWithDeps()
  {
    var services = new ServiceCollection();
    var configBuilder = new ConfigurationBuilder();
    var root = Directory.GetCurrentDirectory();
    var config = configBuilder.AddInMemoryCollection(new[]
                                {
                                    new KeyValuePair<string, string>(HostDefaults.ContentRootKey, root)
                                })
                              .AddJsonFile(Path.Combine("Data", "appsettings.json"))
                              .Build();
    services.AddAsyncRuleEngine<TestInput, TestOutput>()
            .AddScriptedRules<TestInput, TestOutput>(
              config,
              "ofTUWithDeps",
              options => options.AddReferences(typeof(TestDep).Assembly));
    var provider = services.BuildServiceProvider();
    var result = provider.GetService<IRuleEngine<TestInput, TestOutput>>();
    Assert.NotNull(result);
    Assert.Single(result.PreRules);
    Assert.Single(result.Rules);
    Assert.Single(result.PostRules);
    Assert.True(result.IsAsync);
  }
}
