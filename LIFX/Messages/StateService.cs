using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class StateService : LifxMessage, ILifxService {
        public const LifxMessageType TYPE = LifxMessageType.StateService;

        public StateService() : base(TYPE) {

        }

        public StateService(ILifxService service) {
            this.Service = service.Service;
            this.Port = service.Port;
        }

        public LifxService Service { get; set; }
        public uint Port { get; set; }

        protected override void WritePayload(BinaryWriter writer) {
            /* uint8_t service */ writer.Write((byte)this.Service);
            /* uint32_t le port */ writer.Write(this.Port);
        }

        protected override void ReadPayload(BinaryReader reader) {
            this.Service = (LifxService)reader.ReadByte();
            this.Port = reader.ReadUInt32();
        }
    }
}
