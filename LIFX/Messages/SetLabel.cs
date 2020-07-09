using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class SetLabel : LifxMessage, ILifxLabel {
        public const LifxMessageType TYPE = LifxMessageType.SetLabel;

        public SetLabel() : base(TYPE) {

        }

        public string Label { get; set; }

        protected override void WritePayload(BinaryWriter writer) {
            byte[] label = new byte[32];

            Encoding.UTF8.GetBytes(this.Label).CopyTo(label, 0);

            /* uint8_t[32] label */ writer.Write(label, 0, 32);
        }

        protected override void ReadPayload(BinaryReader reader) {
            byte[] label = reader.ReadBytes(32);

            this.Label = Encoding.UTF8.GetString(label.TakeWhile(x => x != 0).ToArray());
        }
    }
}
