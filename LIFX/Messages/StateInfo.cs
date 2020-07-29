// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;
using System.IO;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Sent by a device stating its info.
    /// </summary>
    internal class StateInfo : LifxMessage, ILifxInfo {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateInfo"/> class.
        /// </summary>
        public StateInfo() : base(LifxMessageType.StateInfo) {
            // Empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateInfo"/> class.
        /// </summary>
        /// <param name="info">The <see cref="ILifxInfo"/> to initialize this message from.</param>
        public StateInfo(ILifxInfo info) : this() {
            this.Time = info.Time;
            this.Uptime = info.Uptime;
            this.Downtime = info.Downtime;
        }

        /// <inheritdoc />
        public DateTime Time { get; set; }

        /// <inheritdoc />
        public TimeSpan Uptime { get; set; }

        /// <inheritdoc />
        public TimeSpan Downtime { get; set; }

        /// <inheritdoc />
        protected override void WritePayload(BinaryWriter writer) {
            /* uint64_t le time */ writer.Write(Utilities.DateTimeToNanoseconds(this.Time));
            /* uint64_t le uptime */ writer.Write(Utilities.TimeSpanToNanoseconds(this.Uptime));
            /* uint64_t le downtime */ writer.Write(Utilities.TimeSpanToNanoseconds(this.Downtime));
        }

        /// <inheritdoc />
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
