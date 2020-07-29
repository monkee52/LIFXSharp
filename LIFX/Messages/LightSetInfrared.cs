// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System.IO;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Send this message to alter the current maximum brightness for the infrared channel.
    /// </summary>
    internal class LightSetInfrared : LifxMessage, ILifxInfrared {
        /// <summary>
        /// Initializes a new instance of the <see cref="LightSetInfrared"/> class.
        /// </summary>
        public LightSetInfrared() : base(LifxMessageType.LightSetInfrared) {
            // Empty
        }

        /// <inheritdoc />
        public ushort Level { get; set; }

        /// <inheritdoc />
        protected override void WritePayload(BinaryWriter writer) {
            /* uint16_t le level */ writer.Write(this.Level);
        }

        /// <inheritdoc />
        protected override void ReadPayload(BinaryReader reader) {
            ushort level = reader.ReadUInt16();

            this.Level = level;
        }
    }
}
