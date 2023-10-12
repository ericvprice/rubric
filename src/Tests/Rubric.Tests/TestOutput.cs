using System.Collections.Concurrent;

namespace Rubric.Tests;

public class TestOutput
{
  public bool TestFlag { get; set; }

  public ConcurrentBag<string> Outputs { get; init; } = new();

  public int Counter { get; set; }
}