namespace AydenIO.Lifx {
    /// <summary>
    /// Common properties for <c>Messages.StateVersion</c>
    /// </summary>
    public interface ILifxVersion {
        public uint VendorId { get; }
        public uint ProductId { get; }
        public uint Version { get; }
    }
}