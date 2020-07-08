namespace AydenIO.Lifx {
    public interface ILifxVersion {
        public uint VendorId { get; }
        public uint ProductId { get; }
        public uint Version { get; }
    }
}