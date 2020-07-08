using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class EchoResponse : LifxMessage, ILifxEcho {
        public const LifxMessageType TYPE = LifxMessageType.EchoResponse;

        public EchoResponse() : base(TYPE) {
            this.payload = new byte[64];
        }

        private byte[] payload;

        public IReadOnlyList<byte> GetPayload() {
            return this.payload;
        }

        public void SetPayload(IEnumerable<byte> payload) {
            byte[] payloadBytes = payload.Take(64).ToArray();

            if (payloadBytes.Length != 64) {
                // TODO: ???
            }

            payloadBytes.CopyTo(this.payload, 0);
        }

        protected override void WritePayload(BinaryWriter writer) {
            writer.Write(this.payload);
        }

        protected override void ReadPayload(BinaryReader reader) {
            this.payload = reader.ReadBytes(64);
        }
    }
}
