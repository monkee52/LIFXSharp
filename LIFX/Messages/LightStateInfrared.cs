// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System.IO;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// This message is returned from a GetInfrared message. It indicates the current maximum setting for the infrared channel.
    /// </summary>
    internal class LightStateInfrared : LifxMessage, ILifxInfrared {
        /// <summary>
        /// Initializes a new instance of the <see cref="LightStateInfrared"/> class.
        /// </summary>
        public LightStateInfrared() : base(LifxMessageType.LightStateInfrared) {
            // Empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LightStateInfrared"/> class.
        /// </summary>
        /// <param name="level">The level to initialize this message from.</param>
        public LightStateInfrared(ushort level) : this() {
            this.Level = level;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LightStateInfrared"/> class.
        /// </summary>
        /// <param name="infrared">The <see cref="ILifxInfrared"/> to initialize this message from.</param>
        public LightStateInfrared(ILifxInfrared infrared) : this() {
            this.Level = infrared.Level;
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
