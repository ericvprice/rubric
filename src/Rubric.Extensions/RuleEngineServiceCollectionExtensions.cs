using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rubric.Builder;
using Rubric.Engines.Async;
using Rubric.Extensions.Serialization;
using Rubric.Rules.Scripted;

namespace Rubric.Extensions;

/// <summary>
///   Extension methods for service collection for setting up engines in a DI-compatible fashion for .NET hosting.
/// </summary>
public static class RuleEngineServiceCollectionExtensions
{
  private static ScriptOptions GetDefaultOptions<T>()
   => ScriptOptions.Default
           .WithReferences(typeof(ScriptedRuleContext<T>).Assembly,
                           typeof(EngineContext).Assembly,
                           typeof(ILogger).Assembly,
                           typeof(T).Assembly)
           .AddImports("Rubric",
                       "System.Threading",
                       "System.Threading.Tasks");

  private static ScriptOptions GetDefaultOptions<TIn, TOut>()
    => GetDefaultOptions<TIn>().WithReferences(typeof(TOut).Assembly);

  /// <summary>
  ///   Add dynamically scripted rules.
  /// </summary>
  /// <typeparam name="T">The rule type.</typeparam>
  /// <param name="services">The service collection.</param>
  /// <param name="configuration">The current configuration.</param>
  /// <param name="section">The configuration section to read from.</param>
  /// <param name="setupAction">A callback to configure the script actions.</param>
  /// <returns>The service collection.</returns>
  public static IServiceCollection AddScriptedRules<T>(
      this IServiceCollection services,
      IConfiguration configuration,
      string section,
      Action<ScriptOptions> setupAction = null)
  {
    var model = new AsyncRulesetModel<T>();
    var options = GetDefaultOptions<T>();
    setupAction?.Invoke(options);
    configuration.Bind(section, model);
    model.BasePath = configuration.GetValue<string>(HostDefaults.ContentRootKey);
    var ruleSet = new JsonRuleSet<T>(model, options);
    foreach (var rule in ruleSet.Rules)
      services.AddSingleton(typeof(Rules.Async.IRule<T>), rule);
    return services;
  }

  /// <summary>
  ///   Add dynamically scripted rules.
  /// </summary>
  /// <typeparam name="TIn">The rule input type.</typeparam>
  /// <typeparam name="TOut">The rule output type.</typeparam>
  /// <param name="services">The service collection.</param>
  /// <param name="configuration">The current configuration.</param>
  /// <param name="section">The configuration section to read from.</param>
  /// <param name="setupAction">A callback to configure the script actions.</param>
  /// <returns>The service collection.</returns>
  public static IServiceCollection AddScriptedRules<TIn, TOut>(
      this IServiceCollection services,
      IConfiguration configuration,
      string section,
      Action<ScriptOptions> setupAction = null)
  {
    var model = new AsyncRulesetModel<TIn, TOut>();
    configuration.Bind(section, model);
    model.BasePath = configuration.GetValue<string>(HostDefaults.ContentRootKey);
    var options = GetDefaultOptions<TIn, TOut>();
    setupAction?.Invoke(options);
    var ruleSet = new JsonRuleSet<TIn, TOut>(model, options);
    foreach (var rule in ruleSet.PreRules)
      services.AddSingleton(typeof(Rules.Async.IRule<TIn>), rule);
    foreach (var rule in ruleSet.Rules)
      services.AddSingleton(typeof(Rules.Async.IRule<TIn, TOut>), rule);
    foreach (var rule in ruleSet.PostRules)
      services.AddSingleton(typeof(Rules.Async.IRule<TOut>), rule);
    return services;
  }

  /// <summary>
  ///   Add rules from a given assembly.
  /// </summary>
  /// <typeparam name="T">The rule type to add.</typeparam>
  /// <param name="services">The service collection.</param>
  /// <param name="assembly">The assembly.</param>
  /// <param name="includes">A list of types to include.</param>
  /// <param name="excludes">A list of types to exclude.</param>
  /// <returns>The service collection.</returns>
  public static IServiceCollection AddRules<T>(
      this IServiceCollection services,
      Assembly assembly = null,
      IEnumerable<string> includes = null,
      IEnumerable<string> excludes = null)
  {
    assembly ??= typeof(T).Assembly;
    foreach (var type in assembly.GetTypes<Rules.IRule<T>>(includes, excludes))
      services.AddSingleton(typeof(Rules.IRule<T>), type);
    return services;
  }

  /// <summary>
  ///   Add rules from a given assembly.
  /// </summary>
  /// <typeparam name="TIn">The rule input type to add.</typeparam>
  /// <typeparam name="TOut">The rule output type to add.</typeparam>
  /// <param name="services">The service collection.</param>
  /// <param name="assembly">The assembly.</param>
  /// <param name="includes">A list of types to include.</param>
  /// <param name="excludes">A list of types to exclude.</param>
  /// <returns>The service collection.</returns>
  public static IServiceCollection AddRules<TIn, TOut>(
    this IServiceCollection services,
    Assembly assembly = null,
    string[] includes = null,
    string[] excludes = null)
  {
    assembly ??= typeof(TIn).Assembly;
    includes ??= [];
    excludes ??= [];
    foreach (var type in assembly.GetTypes<Rules.IRule<TIn, TOut>>(includes, excludes))
      services.AddSingleton(typeof(Rules.IRule<TIn, TOut>), type);
    services.AddRules<TIn>(assembly, includes, excludes)
             .AddRules<TOut>(assembly, includes, excludes);
    if (typeof(TOut).Assembly == typeof(TIn).Assembly) return services;
    assembly = typeof(TOut).Assembly;
    foreach (var type in typeof(TOut).Assembly.GetTypes<Rules.IRule<TIn, TOut>>(includes, excludes))
      services.AddSingleton(typeof(Rules.IRule<TIn, TOut>), type);
    services.AddRules<TIn>(assembly, includes, excludes)
             .AddRules<TOut>(assembly, includes, excludes);
    return services;
  }

  /// <summary>
  ///   Add asynchronous rules from a given assembly.
  /// </summary>
  /// <typeparam name="T">The rule input type to add.</typeparam>
  /// <param name="services">The service collection.</param>
  /// <param name="assembly">The assembly.</param>
  /// <param name="includes">A list of types to include.</param>
  /// <param name="excludes">A list of types to exclude.</param>
  /// <returns>The service collection.</returns>
  public static IServiceCollection AddAsyncRules<T>(
       this IServiceCollection services,
       Assembly assembly = null,
       IEnumerable<string> includes = null,
       IEnumerable<string> excludes = null)
  {
    assembly ??= typeof(T).Assembly;
    foreach (var type in assembly.GetTypes<Rules.Async.IRule<T>>(includes, excludes))
      services.AddSingleton(typeof(Rules.Async.IRule<T>), type);
    return services;
  }

  /// <summary>
  ///   Add asynchronous rules from a given assembly.
  /// </summary>
  /// <typeparam name="TIn">The rule input type to add.</typeparam>
  /// <typeparam name="TOut">The rule output type to add.</typeparam>
  /// <param name="services">The service collection.</param>
  /// <param name="assembly">The assembly.</param>
  /// <param name="includes">A list of types to include.</param>
  /// <param name="excludes">A list of types to exclude.</param>
  /// <returns>The service collection.</returns>
  public static IServiceCollection AddAsyncRules<TIn, TOut>(
    this IServiceCollection services,
    Assembly assembly = null,
    string[] includes = null,
    string[] excludes = null)
  {
    assembly ??= typeof(TIn).Assembly;
    includes ??= [];
    excludes ??= [];
    foreach (var type in assembly.GetTypes<Rules.Async.IRule<TIn, TOut>>(includes, excludes))
      services.AddSingleton(typeof(Rules.Async.IRule<TIn, TOut>), type);
    services.AddAsyncRules<TIn>(assembly, includes, excludes)
            .AddAsyncRules<TOut>(assembly, includes, excludes);
    if (typeof(TIn).Assembly == typeof(TOut).Assembly) return services;
    assembly = typeof(TOut).Assembly;
    foreach (var type in assembly.GetTypes<Rules.Async.IRule<TIn, TOut>>(includes, excludes))
      services.AddSingleton(typeof(Rules.Async.IRule<TIn, TOut>), type);
    services.AddAsyncRules<TIn>(assembly, includes, excludes)
            .AddAsyncRules<TOut>(assembly, includes, excludes);
    return services;
  }

  /// <summary>
  ///   Add a rule engine, with the ability to configure it using a builder.
  /// </summary>
  /// <typeparam name="T">The rule input type to add.</typeparam>
  /// <param name="services">The service collection.</param>
  /// <param name="action">The action to configure the engine using an engine builder.</param>
  /// <returns>The service collection.</returns>
  public static IServiceCollection AddRuleEngine<T>(
    this IServiceCollection services,
    Action<IEngineBuilder<T>> action = null)
      where T : class
  {
    if (services is null) throw new ArgumentNullException(nameof(services));
    var builder = EngineBuilder.ForInput<T>();
    action?.Invoke(builder);
    services.AddSingleton(typeof(IEngineBuilder<T>), builder);
    services.AddSingleton<Rubric.Engines.IRuleEngine<T>, DefaultRuleEngine<T>>();
    return services;
  }

  /// <summary>
  ///   Add a rule engine, with the ability to configure it using a builder.
  /// </summary>
  /// <typeparam name="TIn">The rule input type to add.</typeparam>
  /// <typeparam name="TOut">The rule output type to add.</typeparam>
  /// <param name="services">The service collection.</param>
  /// <param name="action">The action to configure the engine using an engine builder.</param>
  /// <returns>The service collection.</returns>
  public static IServiceCollection AddRuleEngine<TIn, TOut>(
    this IServiceCollection services,
    Action<IEngineBuilder<TIn, TOut>> action = null)
    where TIn : class
    where TOut : class
  {
    if (services is null) throw new ArgumentNullException(nameof(services));
    var builder = EngineBuilder.ForInputAndOutput<TIn, TOut>();
    action?.Invoke(builder);
    services.AddSingleton(typeof(IEngineBuilder<TIn, TOut>), builder);
    services.AddSingleton<Engines.IRuleEngine<TIn, TOut>, DefaultRuleEngine<TIn, TOut>>();
    return services;
  }

  /// <summary>
  ///   Add a rule engine, with the ability to configure it using a builder.
  /// </summary>
  /// <typeparam name="T">The rule input type to add.</typeparam>
  /// <param name="services">The service collection.</param>
  /// <param name="action">The action to configure the engine using an engine builder.</param>
  /// <returns>The service collection.</returns>
  public static IServiceCollection AddAsyncRuleEngine<T>(this IServiceCollection services, Action<Builder.Async.IEngineBuilder<T>> action = null)
      where T : class
  {
    if (services is null) throw new ArgumentNullException(nameof(services));
    var builder = EngineBuilder.ForInputAsync<T>();
    action?.Invoke(builder);
    return services.AddSingleton(typeof(Builder.Async.IEngineBuilder<T>), builder)
                   .AddSingleton<IRuleEngine<T>, DefaultAsyncRuleEngine<T>>();
  }
  /// <summary>
  ///   Add a rule engine, with the ability to configure it using a builder.
  /// </summary>
  /// <typeparam name="TIn">The rule input type to add.</typeparam>
  /// <typeparam name="TOut">The rule output type to add.</typeparam>
  /// <param name="services">The service collection.</param>
  /// <param name="action">The action to configure the engine using an engine builder.</param>
  /// <returns>The service collection.</returns>
  public static IServiceCollection AddAsyncRuleEngine<TIn, TOut>(
    this IServiceCollection services,
    Action<Builder.Async.IEngineBuilder<TIn, TOut>> action = null)
      where TIn : class
      where TOut : class
  {
    if (services is null) throw new ArgumentNullException(nameof(services));
    var builder = EngineBuilder.ForInputAndOutputAsync<TIn, TOut>();
    action?.Invoke(builder);
    return services.AddSingleton(typeof(Builder.Async.IEngineBuilder<TIn, TOut>), builder)
                   .AddSingleton<IRuleEngine<TIn, TOut>, DefaultAsyncRuleEngine<TIn, TOut>>();
  }


}
