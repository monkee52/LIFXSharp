using System;

namespace AydenIO.Lifx {
    public interface ILifxHostFirmware {
        public ulong Build { get; }
        public ushort VersionMinor { get; }
        public ushort VersionMajor { get; }
    }
}
