using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class GetWifiState : LifxMessage, ILifxWifiInterface {
        public const LifxMessageType TYPE = LifxMessageType.GetWifiState;

        public LifxWifiInterface Interface { get; set; }

        public GetWifiState() : base(TYPE) {

        }

        protected override void ReadPayload(BinaryReader reader) {
            // Interface
            byte iface = reader.ReadByte();

            this.Interface = (LifxWifiInterface)iface;
        }

        protected override void WritePayload(BinaryWriter writer) {
            /* uint8_t interface */ writer.Write((byte)this.Interface);
        }
    }
}
