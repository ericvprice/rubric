using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Rubric.Scripting
{
  public static class ServiceCollectionExtensions
  {
    public static IServiceCollection AddScriptedRules<T>(
        this ServiceCollection services,
        IConfiguration configuration,
        string section)
    {
      var model = new AsyncRulesetModel<T>();
      configuration.Bind(section, model);
      var ruleSet = new JsonRuleSet<T>(model);
      foreach (var rule in ruleSet.AsyncRules)
        services.AddSingleton(typeof(IAsyncRule<T>), rule);
      return services;
    }

    public static IServiceCollection AddScriptedRules<T, U>(
        this ServiceCollection services,
        IConfiguration configuration,
        string section)
    {
      var model = new AsyncRulesetModel<T, U>();
      configuration.Bind(section, model);
      model.BasePath = configuration.GetValue<string>(HostDefaults.ContentRootKey);
      var ruleSet = new JsonRuleSet<T, U>(model);
      foreach (var rule in ruleSet.AsyncPreRules)
        services.AddSingleton(typeof(IAsyncRule<T>), rule);
      foreach (var rule in ruleSet.AsyncRules)
        services.AddSingleton(typeof(IAsyncRule<T, U>), rule);
      foreach (var rule in ruleSet.AsyncPostRules)
        services.AddSingleton(typeof(IAsyncRule<U>), rule);
      return services;
    }
  }
}