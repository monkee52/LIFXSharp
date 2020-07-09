using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    /// <summary>
    /// Common properties for <c>Messages.SetPower</c>, <c>Messages.StatePower</c>, <c>Messages.LightSetPower</c>, <c>Messages.LightStatePower</c>
    /// </summary>
    public interface ILifxPower {
        public bool PoweredOn { get; set; }
    }
}
