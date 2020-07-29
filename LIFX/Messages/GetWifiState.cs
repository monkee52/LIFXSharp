// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System.IO;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Sent to a device to retrieve the wifi interface state.
    /// </summary>
    internal class GetWifiState : LifxMessage, ILifxWifiInterface {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetWifiState"/> class.
        /// </summary>
        public GetWifiState() : base(LifxMessageType.GetWifiState) {
            // Empty
        }

        /// <inheritdoc />
        public LifxWifiInterface InterfacTypee { get; set; }

        /// <inheritdoc />
        protected override void ReadPayload(BinaryReader reader) {
            // Interface
            byte iface = reader.ReadByte();

            this.InterfacTypee = (LifxWifiInterface)iface;
        }

        /// <inheritdoc />
        protected override void WritePayload(BinaryWriter writer) {
            /* uint8_t interface */ writer.Write((byte)this.InterfacTypee);
        }
    }
}
