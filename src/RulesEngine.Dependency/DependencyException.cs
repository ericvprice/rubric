namespace RulesEngine.Dependency
{
  public class DependencyException : Exception
  {
    public DependencyException(string message = "") : base(message) {
      Details = new List<string>();
    }

    public IEnumerable<string> Details { get; set; }
  }
}