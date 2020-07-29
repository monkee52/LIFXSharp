// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System.IO;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Sent by a device to indicate the services it supports.
    /// </summary>
    internal sealed class StateService : LifxMessage, ILifxService {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateService"/> class.
        /// </summary>
        public StateService() : base(LifxMessageType.StateService) {
            // Empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateService"/> class.
        /// </summary>
        /// <param name="service">The <see cref="ILifxService"/> to initialize this message from.</param>
        public StateService(ILifxService service) : this() {
            this.Service = service.Service;
            this.Port = service.Port;
        }

        /// <inheritdoc />
        public LifxService Service { get; set; }

        /// <inheritdoc />
        public uint Port { get; set; }

        /// <inheritdoc />
        protected override void WritePayload(BinaryWriter writer) {
            /* uint8_t service */ writer.Write((byte)this.Service);
            /* uint32_t le port */ writer.Write(this.Port);
        }

        /// <inheritdoc />
        protected override void ReadPayload(BinaryReader reader) {
            this.Service = (LifxService)reader.ReadByte();
            this.Port = reader.ReadUInt32();
        }
    }
}
