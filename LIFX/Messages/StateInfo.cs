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

        public StateInfo(ILifxInfo info) {
            this.Time = info.Time;
            this.Uptime = info.Uptime;
            this.Downtime = info.Downtime;
        }

        public DateTime Time { get; set; }
        public TimeSpan Uptime { get; set; }
        public TimeSpan Downtime { get; set; }

        protected override void WritePayload(BinaryWriter writer) {
            /* uint64_t le time */ writer.Write(Utilities.DateTimeToNanoseconds(this.Time));
            /* uint64_t le uptime */ writer.Write(Utilities.TimeSpanToNanoseconds(this.Uptime));
            /* uint64_t le downtime */ writer.Write(Utilities.TimeSpanToNanoseconds(this.Downtime));
        }

        protected override void ReadPayload(BinaryReader reader) {
            ulong time = reader.ReadUInt64();

            this.Time = Utilities.NanosecondsToDateTime(time);

            ulong uptime = reader.ReadUInt64();

            this.Uptime = Utilities.NanosecondsToTimeSpan(uptime);

            ulong downtime = reader.ReadUInt64();

            this.Downtime = Utilities.NanosecondsToTimeSpan(downtime);
        }
    }
}
