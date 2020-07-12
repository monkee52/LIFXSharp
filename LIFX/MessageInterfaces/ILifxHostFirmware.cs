using System;

namespace AydenIO.Lifx {
    /// <summary>
    /// Common properties for <c>Messages.StateHostFirmware</c>
    /// </summary>
    public interface ILifxHostFirmware {
        public DateTime Build { get; }
        public ushort VersionMinor { get; }
        public ushort VersionMajor { get; }
    }
}
