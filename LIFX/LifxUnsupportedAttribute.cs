using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class LifxIgnoreUnsupportedAttribute : Attribute {
        public ICollection<string> UnsupportedMethods { get; private set; }

        public LifxIgnoreUnsupportedAttribute(params string[] methodNames) {
            this.UnsupportedMethods = methodNames;
        }
    }
}
