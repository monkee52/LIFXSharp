using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Request an arbitrary payload be echoed back.
    /// </summary>
    internal class EchoRequest : LifxMessage, ILifxEcho {
        public const LifxMessageType TYPE = LifxMessageType.EchoRequest;

        public EchoRequest() : base(TYPE) {
            this.payload = new byte[64];
        }

        private byte[] payload;

        public IReadOnlyList<byte> GetPayload() {
            return this.payload;
        }

        public void SetPayload(IEnumerable<byte> payload) {
            byte[] payloadBytes = payload.Take(64).ToArray();

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
