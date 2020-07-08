using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class StateWifiFirmware : LifxMessage, ILifxWifiFirmware {
        public const LifxMessageType TYPE = LifxMessageType.StateWifiFirmware;

        public StateWifiFirmware() : base(TYPE) {

        }

        public ulong Build { get; set; }
        public ushort VersionMinor { get; set; }
        public ushort VersionMajor { get; set; }

        protected override void WritePayload(BinaryWriter writer) {
            throw new NotSupportedException();
        }

        protected override void ReadPayload(BinaryReader reader) {
            ulong build = reader.ReadUInt64();

            this.Build = build;

            _ = reader.ReadUInt64();

            ushort minor = reader.ReadUInt16();

            this.VersionMinor = minor;

            ushort major = reader.ReadUInt16();

            this.VersionMajor = major;
        }
    }
}
