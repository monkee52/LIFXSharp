﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class StateInfo : LifxMessage, ILifxInfo {
        public const LifxMessageType TYPE = LifxMessageType.StateInfo;

        public StateInfo() : base(TYPE) {

        }

        public DateTime Time { get; set; }
        public TimeSpan Uptime { get; set; }
        public TimeSpan Downtime { get; set; }

        protected override void WritePayload(BinaryWriter writer) {
            throw new NotSupportedException();
        }

        protected override void ReadPayload(BinaryReader reader) {
            ulong time = reader.ReadUInt64();

            this.Time = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) + TimeSpan.FromMilliseconds(time / 1000000);

            ulong uptime = reader.ReadUInt64();

            this.Uptime = TimeSpan.FromMilliseconds(uptime / 1000000);

            ulong downtime = reader.ReadUInt64();

            this.Downtime = TimeSpan.FromMilliseconds(downtime / 1000000);
        }
    }
}
