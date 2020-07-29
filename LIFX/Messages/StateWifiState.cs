// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System.IO;
using System.Net;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Represents a StateWifiState packet.
    /// </summary>
    internal sealed class StateWifiState : LifxMessage, ILifxWifiState {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateWifiState"/> class.
        /// </summary>
        public StateWifiState() : base(LifxMessageType.StateWifiState) {
            // Empty
        }

        /// <inheritdoc />
        public LifxWifiInterface InterfacTypee { get; set; }

        /// <inheritdoc />
        public LifxWifiStatus Status { get; set; }

        /// <inheritdoc />
        public IPAddress IPAddressV4 { get; set; }

        /// <inheritdoc />
        public IPAddress IPAddressV6 { get; set; }

        /// <inheritdoc />
        protected override void ReadPayload(BinaryReader reader) {
            // Interface
            byte iface = reader.ReadByte();

            this.InterfacTypee = (LifxWifiInterface)iface;

            // Status
            byte status = reader.ReadByte();

            this.Status = (LifxWifiStatus)status;

            // IPv4 address
            byte[] address4 = reader.ReadBytes(4);

            this.IPAddressV4 = new IPAddress(address4);

            // IPv6 address
            byte[] address6 = reader.ReadBytes(16);

            this.IPAddressV6 = new IPAddress(address6);
        }

        /// <inheritdoc />
        protected override void WritePayload(BinaryWriter writer) {
            /* uint8_t interface */ writer.Write((byte)this.InterfacTypee);
            /* uint8_t status */ writer.Write((byte)this.Status);

            byte[] ipv4Bytes = new byte[4];

            this.IPAddressV4.TryWriteBytes(ipv4Bytes, out _);

            /* uint8_t[4] ipv4_address */ writer.Write(ipv4Bytes);

            byte[] ipv6Bytes = new byte[16];

            this.IPAddressV6.TryWriteBytes(ipv6Bytes, out _);

            /* uint8_t[16] ipv6_address */ writer.Write(ipv6Bytes);
        }
    }
}
