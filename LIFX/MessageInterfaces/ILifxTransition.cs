using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    /// <summary>
    /// Common properties for transitionable messages <c>Messages.LightSetPower</c> and <c>Messages.LightSetColor</c>
    /// </summary>
    public interface ILifxTransition {
        /// <value>The duration of the transition</value>
        public TimeSpan Duration { get; }
    }
}
