namespace AydenIO.Lifx {
    /// <summary>
    /// Common properties for <c>Messages.StateVersion</c>
    /// </summary>
    public interface ILifxVersion {
        /// <value>The vendor identifier</value>
        public uint VendorId { get; }

        /// <value>The product identifier</value>
        public uint ProductId { get; }

        /// <value>The hardware version</value>
        public uint Version { get; }
    }
}