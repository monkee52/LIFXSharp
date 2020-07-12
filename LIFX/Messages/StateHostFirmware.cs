﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class StateHostFirmware : LifxMessage, ILifxHostFirmware {
        public const LifxMessageType TYPE = LifxMessageType.StateHostFirmware;

        public StateHostFirmware() : base(TYPE) {

        }

        public DateTime Build { get; set; }
        public ushort VersionMinor { get; set; }
        public ushort VersionMajor { get; set; }

        protected override void WritePayload(BinaryWriter writer) {
            TimeSpan build = this.Build - DateTime.UnixEpoch;

            /* uint64_t le build */ writer.Write(Utilities.DateTimeToNanoseconds(this.Build));
            /* uint16_t le minor */ writer.Write(this.VersionMinor);
            /* uint16_t le major */ writer.Write(this.VersionMajor);
        }

        protected override void ReadPayload(BinaryReader reader) {
            ulong build = reader.ReadUInt64();

            this.Build = Utilities.NanosecondsToDateTime(build);

            _ = reader.ReadUInt64();

            ushort minor = reader.ReadUInt16();

            this.VersionMinor = minor;

            ushort major = reader.ReadUInt16();

            this.VersionMajor = major;
        }
    }
}
