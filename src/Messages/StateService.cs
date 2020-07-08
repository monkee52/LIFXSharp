using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class StateService : LifxMessage, ILifxService {
        public const LifxMessageType TYPE = LifxMessageType.StateService;

        public StateService() : base(TYPE) {

        }

        public LifxService Service { get; set; }
        public uint Port { get; set; }

        protected override void WritePayload(BinaryWriter writer) {
            throw new NotSupportedException();
        }

        protected override void ReadPayload(BinaryReader reader) {
            this.Service = (LifxService)reader.ReadByte();
            this.Port = reader.ReadUInt32();
        }
    }
}
