using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    public interface ILifxTag : ILifxTagId {
        public string Label { get; }
    }
}
