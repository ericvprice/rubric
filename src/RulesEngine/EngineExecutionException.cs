using System;

namespace RulesEngine
{
    [Serializable]
    public class EngineExecutionException : Exception
    {
        public EngineExecutionException(string message, Exception innerException) : base(message, innerException) { }

        public IEngineContext Context { get; internal set; }

        public object Input { get; internal set; }

        public object Output { get; internal set; }

        public object Rule { get; internal set; }
    }
}