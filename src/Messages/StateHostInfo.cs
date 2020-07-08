using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class StateHostInfo : LifxMessage, ILifxHostInfo {
        public const LifxMessageType TYPE = LifxMessageType.StateHostInfo;

        public StateHostInfo() : base(TYPE) {

        }

        public float Signal { get; set; }
        public uint TransmittedBytes { get; set; }
        public uint ReceivedBytes { get; set; }

        protected override void WritePayload(BinaryWriter writer) {
            /* float32 le signal */ writer.Write(this.Signal);
            /* uint32_t le tx */ writer.Write(this.TransmittedBytes);
            /* uint32_t le rx */ writer.Write(this.ReceivedBytes);
        }

        protected override void ReadPayload(BinaryReader reader) {
            float signal = reader.ReadSingle();

            this.Signal = signal;

            uint tx = reader.ReadUInt32();

            this.TransmittedBytes = tx;

            uint rx = reader.ReadUInt32();

            this.ReceivedBytes = rx;

            _ = reader.ReadInt16();
        }
    }
}
