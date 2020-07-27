using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class StateWifiState : LifxMessage, ILifxWifiState {
        public const LifxMessageType TYPE = LifxMessageType.StateWifiState;

        public LifxWifiInterface Interface { get; set; }

        public LifxWifiStatus Status { get; set; }

        public IPAddress IPAddressV4 { get; set; }

        public IPAddress IPAddressV6 { get; set; }

        public StateWifiState() : base(TYPE) {

        }

        protected override void ReadPayload(BinaryReader reader) {
            // Interface
            byte iface = reader.ReadByte();

            this.Interface = (LifxWifiInterface)iface;

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

        protected override void WritePayload(BinaryWriter writer) {
            /* uint8_t interface */ writer.Write((byte)this.Interface);
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
