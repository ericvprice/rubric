namespace RulesEngine.Dependency
{
    public class DependencyException : Exception
    {
        public DependencyException(string message) : base(message) { }

        public IEnumerable<string> Details { get; set; }
    }
}