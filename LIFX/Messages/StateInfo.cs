using System;
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
            ulong time = (ulong)(this.Time - LifxNetwork.UNIX_EPOCH).Ticks * 100;

            /* uint64_t le time */ writer.Write(time);

            ulong uptime = (ulong)this.Uptime.Ticks * 100;

            /* uint64_t le uptime */ writer.Write(uptime);

            ulong downtime = (ulong)this.Downtime.Ticks * 100;

            /* uint64_t le downtime */ writer.Write(downtime);
        }

        protected override void ReadPayload(BinaryReader reader) {
            ulong time = reader.ReadUInt64();

            this.Time = LifxNetwork.UNIX_EPOCH + TimeSpan.FromTicks((long)(time / 100));

            ulong uptime = reader.ReadUInt64();

            this.Uptime = TimeSpan.FromTicks((long)(uptime / 100));

            ulong downtime = reader.ReadUInt64();

            this.Downtime = TimeSpan.FromTicks((long)(downtime / 100));
        }
    }
}
