using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class StateLabel : LifxMessage, ILifxLabel {
        public const LifxMessageType TYPE = LifxMessageType.StateLabel;

        public StateLabel() : base(TYPE) {

        }

        public string Label { get; set; }

        protected override void WritePayload(BinaryWriter writer) {
            writer.Write(Utilities.StringToFixedBuffer(this.Label, 32));
        }

        protected override void ReadPayload(BinaryReader reader) {
            byte[] label = reader.ReadBytes(32);

            this.Label = Utilities.BufferToString(label);
        }
    }
}
