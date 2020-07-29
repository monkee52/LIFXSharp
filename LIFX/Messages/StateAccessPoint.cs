// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System.IO;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Sent from a device stating an access point the device can see.
    /// </summary>
    internal class StateAccessPoint : LifxMessage, ILifxAccessPoint {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateAccessPoint"/> class.
        /// </summary>
        public StateAccessPoint() : base(LifxMessageType.StateAccessPoint) {
            // Empty
        }

        /// <inheritdoc />
        public LifxWifiInterface InterfacTypee { get; set; }

        /// <inheritdoc />
        public string Ssid { get; set; }

        /// <inheritdoc />
        public LifxSecurityProtocol SecurityProtocol { get; set; }

        /// <inheritdoc />
        public ushort Strength { get; set; }

        /// <inheritdoc />
        public ushort Channel { get; set; }

        /// <inheritdoc />
        protected override void ReadPayload(BinaryReader reader) {
            byte iface = reader.ReadByte();

            this.InterfacTypee = (LifxWifiInterface)iface;

            // SSID
            byte[] ssid = reader.ReadBytes(32);

            this.Ssid = Utilities.BufferToString(ssid);

            // Security protocol
            byte securityProtocol = reader.ReadByte();

            this.SecurityProtocol = (LifxSecurityProtocol)securityProtocol;

            // Strength
            ushort strength = reader.ReadUInt16();

            this.Strength = strength;

            // Channel
            ushort channel = reader.ReadUInt16();

            this.Channel = channel;
        }

        /// <inheritdoc />
        protected override void WritePayload(BinaryWriter writer) {
            /* uint8_t interface */ writer.Write((byte)this.InterfacTypee);
            /* uint8_t[32] ssid */ writer.Write(Utilities.StringToFixedBuffer(this.Ssid, 32));
            /* uint8_t security_protocol */ writer.Write((byte)this.SecurityProtocol);
            /* uint16_t le strength */ writer.Write(this.Strength);
            /* uint16_t le channel */ writer.Write(this.Channel);
        }
    }
}
