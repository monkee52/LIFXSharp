using System;

namespace AydenIO.Lifx {
    /// <summary>
    /// Common properties for <c>Messages.StateWifiFirmware</c>
    /// </summary>
    public interface ILifxWifiFirmware {
        /// <value>Firmware build time</value>
        public DateTime Build { get; }

        /// <value>Firmware minor version</value>
        public ushort VersionMinor { get; }

        /// <value>Firmware major version</value>
        public ushort VersionMajor { get; }
    }
}