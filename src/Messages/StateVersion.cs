using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class StateVersion : LifxMessage, ILifxVersion {
        public const LifxMessageType TYPE = LifxMessageType.StateVersion;

        public StateVersion() : base(TYPE) {

        }

        public uint VendorId { get; private set; }

        public uint ProductId { get; private set; }

        public uint Version { get; private set; }

        protected override void WritePayload(BinaryWriter writer) {
            throw new NotSupportedException();
        }

        protected override void ReadPayload(BinaryReader reader) {
            uint vendor = reader.ReadUInt32();

            this.VendorId = vendor;

            uint product = reader.ReadUInt32();

            this.ProductId = product;

            uint version = reader.ReadUInt32();

            this.Version = version;
        }
    }
}
