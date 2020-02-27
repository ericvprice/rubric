using System.Collections.Generic;

namespace RulesEngine
{
    public class EngineContext : IEngineContext
    {
        private readonly Dictionary<string, object> _stash = new Dictionary<string, object>();

        public object this[string name]
        {
            get => _stash[name];
            set => _stash[name] = value;
        }

        public bool ContainsKey(string name) => _stash.ContainsKey(name);

        public T Get<T>(string name) => (T) _stash[name];

        public void Remove(string name) => _stash.Remove(name);

        public EngineContext Clone()
        {
            var toReturn = new EngineContext();
            foreach (var name in _stash.Keys)
                toReturn._stash[name] = _stash[name];
            return toReturn;
        }
    }
}