using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    /// <summary>
    /// Common properties for <c>Messages.SetPower</c>, <c>Messages.StatePower</c>, <c>Messages.LightSetPower</c>, <c>Messages.LightStatePower</c>
    /// </summary>
    public interface ILifxPower {
        /// <value>Whether the device is powered on</value>
        public bool PoweredOn { get; set; }
    }
}
