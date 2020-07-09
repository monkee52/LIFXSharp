namespace AydenIO.Lifx {
    /// <summary>
    /// Common properties for <c>Messages.StateWifiInfo</c>
    /// </summary>
    public interface ILifxWifiInfo {
        public float Signal { get; }
        public uint TransmittedBytes { get; }
        public uint ReceivedBytes { get; }
    }
}