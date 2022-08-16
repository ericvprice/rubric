using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Rubric;
using Rubric.Builder;
using Rubric.Extensions;
using Rubric.Extensions.Serialization;
using Rubric.Rules;
using Rubric.Rules.Async;

namespace Microsoft.Extensions.DependencyInjection;

public static class RuleEngineServiceCollectionExtensions
{
  public static IServiceCollection AddScriptedRules<T>(
      this IServiceCollection services,
      IConfiguration configuration,
      string section)
  {
    var model = new AsyncRulesetModel<T>();
    configuration.Bind(section, model);
    model.BasePath = configuration.GetValue<string>(HostDefaults.ContentRootKey);
    var ruleSet = new JsonRuleSet<T>(model);
    foreach (var rule in ruleSet.AsyncRules)
      services.AddSingleton(typeof(IAsyncRule<T>), rule);
    return services;
  }

  public static IServiceCollection AddScriptedRules<T, U>(
      this IServiceCollection services,
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

  public static IServiceCollection AddRules<T>(
      this IServiceCollection services,
      Assembly assembly = null,
      string[] includes = null,
      string[] excludes = null)
  {
    assembly ??= typeof(T).Assembly;
    foreach (var type in assembly.GetTypes<IRule<T>>(includes, excludes))
      services.AddSingleton(typeof(IRule<T>), type);
    return services;
  }

  public static IServiceCollection AddRules<T, U>(
    this IServiceCollection services,
    Assembly assembly = null,
    string[] includes = null,
    string[] excludes = null)
  {
    assembly ??= typeof(T).Assembly;
    includes ??= new string[] { };
    excludes ??= new string[] { };
    foreach (var type in assembly.GetTypes<IRule<T, U>>(includes, excludes))
      services.AddSingleton(typeof(IRule<T, U>), type);
    services.AddRules<T>(assembly, includes, excludes)
             .AddRules<U>(assembly, includes, excludes);
    if (typeof(U).Assembly == typeof(T).Assembly) return services;
    assembly = typeof(U).Assembly;
    foreach (var type in typeof(U).Assembly.GetTypes<IRule<T, U>>(includes, excludes))
      services.AddSingleton(typeof(IRule<T, U>), type);
    services.AddRules<T>(assembly, includes, excludes)
             .AddRules<U>(assembly, includes, excludes);
    return services;
  }

  public static IServiceCollection AddAsyncRules<T>(
       this IServiceCollection services,
       Assembly assembly = null,
       string[] includes = null,
       string[] excludes = null)
  {
    assembly ??= typeof(T).Assembly;
    foreach (var type in assembly.GetTypes<IAsyncRule<T>>(includes, excludes))
      services.AddSingleton(typeof(IAsyncRule<T>), type);
    return services;
  }

  public static IServiceCollection AddAsyncRules<T, U>(
    this IServiceCollection services,
    Assembly assembly = null,
    string[] includes = null,
    string[] excludes = null)
  {
    assembly ??= typeof(T).Assembly;
    includes ??= new string[] { };
    excludes ??= new string[] { };
    foreach (var type in assembly.GetTypes<IAsyncRule<T, U>>(includes, excludes))
      services.AddSingleton(typeof(IAsyncRule<T, U>), type);
    services.AddAsyncRules<T>(assembly, includes, excludes)
            .AddAsyncRules<U>(assembly, includes, excludes);
    if (typeof(T).Assembly == typeof(U).Assembly) return services;
    assembly = typeof(U).Assembly;
    foreach (var type in assembly.GetTypes<IAsyncRule<T, U>>(includes, excludes))
      services.AddSingleton(typeof(IAsyncRule<T, U>), type);
    services.AddAsyncRules<T>(assembly, includes, excludes)
            .AddAsyncRules<U>(assembly, includes, excludes);
    return services;
  }

  public static IServiceCollection AddRuleEngine<T>(
    this IServiceCollection services,
    Action<IEngineBuilder<T>> action = null)
      where T : class
  {
    if (services is null) throw new ArgumentNullException(nameof(services));
    var builder = EngineBuilder.ForInput<T>();
    action?.Invoke(builder);
    services.AddSingleton(typeof(IEngineBuilder<T>), builder);
    services.AddSingleton<IRuleEngine<T>, DefaultRuleEngine<T>>();
    return services;
  }

  public static IServiceCollection AddRuleEngine<T, U>(
    this IServiceCollection services,
    Action<IEngineBuilder<T, U>> action = null)
    where T : class
    where U : class
  {
    if (services is null) throw new ArgumentNullException(nameof(services));
    var builder = EngineBuilder.ForInputAndOutput<T, U>();
    action?.Invoke(builder);
    services.AddSingleton(typeof(IEngineBuilder<T, U>), builder);
    services.AddSingleton<IRuleEngine<T, U>, DefaultRuleEngine<T, U>>();
    return services;
  }

  public static IServiceCollection AddAsyncRuleEngine<T>(this IServiceCollection services, Action<IAsyncEngineBuilder<T>> action = null)
      where T : class
  {
    if (services is null) throw new ArgumentNullException(nameof(services));
    var builder = EngineBuilder.ForInputAsync<T>();
    action?.Invoke(builder);
    return services.AddSingleton(typeof(IAsyncEngineBuilder<T>), builder)
                   .AddSingleton<IAsyncRuleEngine<T>, DefaultAsyncRuleEngine<T>>();
  }

  public static IServiceCollection AddAsyncRuleEngine<T, U>(
    this IServiceCollection services,
    Action<IAsyncEngineBuilder<T, U>> action = null)
      where T : class
      where U : class
  {
    if (services is null) throw new ArgumentNullException(nameof(services));
    var builder = EngineBuilder.ForInputAndOutputAsync<T, U>();
    action?.Invoke(builder);
    return services.AddSingleton(typeof(IAsyncEngineBuilder<T, U>), builder)
                   .AddSingleton<IAsyncRuleEngine<T, U>, DefaultAsyncRuleEngine<T, U>>();
  }


}
