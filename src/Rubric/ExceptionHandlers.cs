namespace Rubric;

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
          (e, _, _, _, _)
              => throw new EngineHaltException("Uncaught exception.", e));

  /// <summary>
  ///     Halt item on uncaught exceptions.
  /// </summary>
  /// <value>A static reusable exception handler.</value>
  public static IExceptionHandler HaltItem { get; }
      = new LambdaExceptionHandler(
          (e, _, _, _, _)
              => throw new ItemHaltException("Uncaught exception.", e));

  /// <summary>
  ///     Let the exception bubble out of the engine.
  /// </summary>
  /// <value>A static reusable exception handler.</value>
  public static IExceptionHandler Rethrow { get; }
      = new LambdaExceptionHandler((_, _, _, _, _) => false);

  /// <summary>
  ///     Let the exception bubble out of the engine.
  /// </summary>
  /// <value>A static reusable exception handler.</value>
  public static IExceptionHandler Ignore { get; }
      = new LambdaExceptionHandler((_, _, _, _, _) => true);
}