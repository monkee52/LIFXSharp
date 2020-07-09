namespace AydenIO.Lifx {
    /// <summary>
    /// Common properties for <c>Messages.StateWifiFirmware</c>
    /// </summary>
    public interface ILifxWifiFirmware {
        public ulong Build { get; }
        public ushort VersionMinor { get; }
        public ushort VersionMajor { get; }
    }
}