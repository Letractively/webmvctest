using System;
using System.Collections.Generic;

namespace WebMVCTest.Model
{
    public class Context
    {
        private Dictionary<string, string> variables = new Dictionary<string, string>();

        public Context()
        {
        }

        public void Add(string key, string value)
        {
            if (this.variables.ContainsKey(key))
            {
                this.variables[key] = value;
            }
            else
            {
                this.variables.Add(key, value);
            }
        }

        public bool Contains(string key)
        {
            return this.variables.ContainsKey(key);
        }

        public string Get(string key)
        {
            return this.variables[key];
        }

        public void Remove(string key)
        {
            this.variables.Remove(key);
        }

        public void Clear()
        {
            this.variables.Clear();
        }

        public IResolver GetResolver()
        {
            return new ContextResolver(this);
        }

        public Context Copy()
        {
            Context copy = new Context();
            
            foreach (string key in this.variables.Keys) 
            {
                copy.Add(key, this.variables[key]);
            }

            return copy;
        }
    }
}

