using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class StateAccessPoint : LifxMessage, ILifxAccessPoint {
        public const LifxMessageType TYPE = LifxMessageType.StateAccessPoint;

        public LifxWifiInterface Interface { get; set; }

        public string Ssid { get; set; }

        public LifxSecurityProtocol SecurityProtocol { get; set; }

        public ushort Strength { get; set; }

        public ushort Channel { get; set; }


        public StateAccessPoint() : base(TYPE) {

        }

        protected override void ReadPayload(BinaryReader reader) {
            byte iface = reader.ReadByte();

            this.Interface = (LifxWifiInterface)iface;

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

        protected override void WritePayload(BinaryWriter writer) {
            /* uint8_t interface */ writer.Write((byte)this.Interface);
            /* uint8_t[32] ssid */ writer.Write(Utilities.StringToFixedBuffer(this.Ssid, 32));
            /* uint8_t security_protocol */ writer.Write((byte)this.SecurityProtocol);
            /* uint16_t le strength */ writer.Write(this.Strength);
            /* uint16_t le channel */ writer.Write(this.Channel);
        }
    }
}
