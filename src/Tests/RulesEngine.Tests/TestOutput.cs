using System.Collections.Generic;

namespace RulesEngine.Tests
{
    public class TestOutput
    {
        public bool TestFlag { get; set; }

        public List<string> Outputs { get; } = new List<string>();
    }
}