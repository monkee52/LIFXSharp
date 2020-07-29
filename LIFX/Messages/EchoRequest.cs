// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Request an arbitrary payload be echoed back.
    /// </summary>
    internal class EchoRequest : LifxMessage, ILifxEcho {
        private byte[] payload;

        /// <summary>
        /// Initializes a new instance of the <see cref="EchoRequest"/> class.
        /// </summary>
        public EchoRequest() : base(LifxMessageType.EchoRequest) {
            this.payload = new byte[64];
        }

        /// <inheritdoc />
        public IReadOnlyList<byte> GetPayload() {
            return this.payload;
        }

        /// <inheritdoc />
        public void SetPayload(IEnumerable<byte> payload) {
            byte[] payloadBytes = payload.Take(64).ToArray();

            payloadBytes.CopyTo(this.payload, 0);
        }

        /// <inheritdoc />
        protected override void WritePayload(BinaryWriter writer) {
            writer.Write(this.payload);
        }

        /// <inheritdoc />
        protected override void ReadPayload(BinaryReader reader) {
            this.payload = reader.ReadBytes(64);
        }
    }
}
