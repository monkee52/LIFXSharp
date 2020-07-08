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
            byte[] label = new byte[32];

            Encoding.UTF8.GetBytes(this.Label).CopyTo(label, 0);

            writer.Write(label);
        }

        protected override void ReadPayload(BinaryReader reader) {
            byte[] label = reader.ReadBytes(32);

            this.Label = Encoding.UTF8.GetString(label.TakeWhile(x => x != 0).ToArray());
        }
    }
}
