using System;

namespace AydenIO.Lifx {
    public interface ILifxHostInfo {
        public float Signal { get; set; }
        public uint TransmittedBytes { get; set; }
        public uint ReceivedBytes { get; set; }
    }
}