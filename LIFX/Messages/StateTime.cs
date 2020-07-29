// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;
using System.IO;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Sent by a device to state the current time.
    /// </summary>
    internal class StateTime : LifxMessage, ILifxTime {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateTime"/> class.
        /// </summary>
        public StateTime() : base(LifxMessageType.StateTime) {
            // Empty
        }

        /// <inheritdoc />
        public DateTime Time { get; set; }

        /// <inheritdoc />
        protected override void ReadPayload(BinaryReader reader) {
            // Time
            ulong time = reader.ReadUInt64();

            this.Time = Utilities.NanosecondsToDateTime(time * 1000uL);
        }

        /// <inheritdoc />
        protected override void WritePayload(BinaryWriter writer) {
            /* uint64_t le time */ writer.Write(Utilities.DateTimeToNanoseconds(this.Time) / 1000uL);
        }
    }
}
