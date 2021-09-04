namespace Rubric.Tests.Core;
public class ExceptionHandlerTests
{
  [Fact]
  public void NullConstructorParamterThrows()
  {
    Assert.Throws<ArgumentNullException>(() => new LambdaExceptionHandler(null));
  }
}
