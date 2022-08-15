using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Rubric.Builder;
using Rubric.Rules;
using Rubric.Rules.Async;

namespace Rubric.Extensions;

public static class RuleEngineExtensions
{

  public static IServiceCollection AddRules<T>(
      this IServiceCollection services,
      Assembly assembly = null,
      string[] includes = null,
      string[] excludes = null)
  {
    assembly ??= Assembly.GetCallingAssembly();
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
    assembly ??= Assembly.GetCallingAssembly();
    includes ??= new string[] { };
    excludes ??= new string[] { };
    foreach (var type in assembly.GetTypes<IRule<T, U>>(includes, excludes))
      services.AddSingleton(typeof(IRule<T, U>), type);
    return services.AddRules<T>(assembly, includes, excludes)
                   .AddRules<U>(assembly, includes, excludes);
  }

  public static IServiceCollection AddAsyncRules<T>(
       this IServiceCollection services,
       Assembly assembly = null,
       string[] includes = null,
       string[] excludes = null)
  {
    assembly ??= Assembly.GetCallingAssembly();
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
    assembly ??= Assembly.GetCallingAssembly();
    includes ??= new string[] { };
    excludes ??= new string[] { };
    foreach (var type in assembly.GetTypes<IAsyncRule<T, U>>(includes, excludes))
      services.AddSingleton(typeof(IAsyncRule<T, U>), type);
    return services.AddRules<T>(assembly, includes, excludes)
                   .AddRules<U>(assembly, includes, excludes);
  }

  public static IServiceCollection AddRuleEngine<T>(
    this IServiceCollection services,
    Action<IEngineBuilder<T>> action = null)
      where T : class
  {
    if (services is null) throw new ArgumentNullException(nameof(services));
    services.AddOptions();
    var builder = EngineBuilder.ForInput<T>();
    action?.Invoke(builder);
    services.AddSingleton(typeof(IEngineBuilder<T>), builder);
    services.AddSingleton<IRuleEngine<T>, DefaultRuleEngine<T>>();
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
