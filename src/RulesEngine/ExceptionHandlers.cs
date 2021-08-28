namespace RulesEngine;

/// <summary>
///     Convenience unhandled exception error policies;
/// </summary>
public static class ExceptionHandlers
{

  /// <summary>
  ///     Halt engine on uncaught exceptions.
  /// </summary>
  /// <value>A static reusable exception handler.</value>
  public static IExceptionHandler HaltEngine { get; }
      = new LambdaExceptionHandler(
          (e, c, i, o, r)
              => throw new EngineHaltException("Uncaught exception.", e));

  /// <summary>
  ///     Halt item on uncaught exceptions.
  /// </summary>
  /// <value>A static reusable exception handler.</value>
  public static IExceptionHandler HaltItem { get; }
      = new LambdaExceptionHandler(
          (e, c, i, o, r)
              => throw new ItemHaltException("Uncaught exception.", e));

  /// <summary>
  ///     Let the exception bubble out of the engine.
  /// </summary>
  /// <value>A static reusable exception handler.</value>
  public static IExceptionHandler Throw { get; }
      = new LambdaExceptionHandler((e, c, i, o, r) => false);

  /// <summary>
  ///     Let the exception bubble out of the engine.
  /// </summary>
  /// <value>A static reusable exception handler.</value>
  public static IExceptionHandler Ignore { get; }
      = new LambdaExceptionHandler((e, c, i, o, r) => true);

}