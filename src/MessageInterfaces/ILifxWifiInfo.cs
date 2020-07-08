namespace AydenIO.Lifx {
    public interface ILifxWifiInfo {
        public float Signal { get; }
        public uint TransmittedBytes { get; }
        public uint ReceivedBytes { get; }
    }
}