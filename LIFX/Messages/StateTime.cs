using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class StateTime : LifxMessage, ILifxTime {
        public const LifxMessageType TYPE = LifxMessageType.StateTime;

        public DateTime Time { get; set; }

        public StateTime() : base(TYPE) {

        }

        protected override void ReadPayload(BinaryReader reader) {
            // Time
            ulong time = reader.ReadUInt64();

            this.Time = Utilities.NanosecondsToDateTime(time * 1000uL);
        }

        protected override void WritePayload(BinaryWriter writer) {
            /* uint64_t le time */ writer.Write(Utilities.DateTimeToNanoseconds(this.Time) / 1000uL);
        }
    }
}
