using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    public interface ILifxTransition {
        public TimeSpan Duration { get; set; }
    }
}
