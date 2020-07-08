namespace AydenIO.Lifx {
    public interface ILifxWifiFirmware {
        public ulong Build { get; }
        public ushort VersionMinor { get; }
        public ushort VersionMajor { get; }
    }
}