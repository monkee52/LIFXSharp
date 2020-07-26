using System;

namespace AydenIO.Lifx {
    /// <summary>
    /// Common properties for <c>Messages.StateHostInfo</c>
    /// </summary>
    public interface ILifxHostInfo {
        /// <value>Radio receive signal strength</value>
        public float Signal { get; }

        /// <value>Transmitted bytes since power on</value>
        public uint TransmittedBytes { get; }

        /// <value>Received bytes since power on</value>
        public uint ReceivedBytes { get; }

        /// <summary>
        /// Returns the signal strength as a normalised value.
        /// </summary>
        /// <returns>The quality of the received signal</returns>
        public LifxSignalStrength GetSignalStrength();
    }
}