using System;

namespace AydenIO.Lifx {
    /// <summary>
    /// Common properties for <c>Messages.StateHostInfo</c>
    /// </summary>
    public interface ILifxHostInfo {
        public float Signal { get; set; }
        public uint TransmittedBytes { get; set; }
        public uint ReceivedBytes { get; set; }

        public LifxSignalStrength GetSignalStrength();
    }
}