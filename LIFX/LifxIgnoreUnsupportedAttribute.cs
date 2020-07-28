// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;
using System.Collections.Generic;

namespace AydenIO.Lifx {
    /// <summary>
    /// Used to explicitly allow a method to call an unsupported device method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class LifxIgnoreUnsupportedAttribute : Attribute {
        public LifxIgnoreUnsupportedAttribute(params string[] methodNames) {
            this.UnsupportedMethods = methodNames;
        }

        public ICollection<string> UnsupportedMethods { get; private set; }
    }
}
