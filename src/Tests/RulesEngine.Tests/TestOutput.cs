namespace RulesEngine.Tests
{
  public class TestOutput
  {
    public bool TestFlag { get; set; }

    public List<string> Outputs { get; set; } = new List<string>();
    public int Counter { get; internal set; }
  }
}