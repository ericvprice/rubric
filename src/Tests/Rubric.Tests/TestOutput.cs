namespace Rubric.Tests;

public class TestOutput
{
  public bool TestFlag { get; set; }

  public List<string> Outputs { get; set; } = new();
  public int Counter { get; internal set; }
}