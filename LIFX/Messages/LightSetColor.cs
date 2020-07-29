// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;
using System.IO;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Sent by a client to change the light state.
    /// </summary>
    internal sealed class LightSetColor : LifxMessage, ILifxHsbkColor, ILifxTransition {
        /// <summary>
        /// Initializes a new instance of the <see cref="LightSetColor"/> class.
        /// </summary>
        public LightSetColor() : base(LifxMessageType.LightSetColor) {
            // Empty
        }

        /// <inheritdoc />
        public ushort Hue { get; set; }

        /// <inheritdoc />
        public ushort Saturation { get; set; }

        /// <inheritdoc />
        public ushort Brightness { get; set; }

        /// <inheritdoc />
        public ushort Kelvin { get; set; }

        /// <inheritdoc />
        public TimeSpan Duration { get; set; }

        /// <inheritdoc />
        public void FromHsbk(ILifxHsbkColor hsbk) {
            if (hsbk != this && hsbk is not null) {
                this.Hue = hsbk.Hue;
                this.Saturation = hsbk.Saturation;
                this.Brightness = hsbk.Brightness;
                this.Kelvin = hsbk.Kelvin;
            }
        }

        /// <inheritdoc />
        public ILifxHsbkColor ToHsbk() {
            return this;
        }

        /// <inheritdoc />
        protected override void WritePayload(BinaryWriter writer) {
            /* uint8_t reserved */ writer.Write((byte)0);

            // HSBK
            /* uint16_t le hue */ writer.Write(this.Hue);
            /* uint16_t le saturation */ writer.Write(this.Saturation);
            /* uint16_t le brightness */ writer.Write(this.Brightness);
            /* uint16_t le kelvin */ writer.Write(this.Kelvin);

            /* uint32_t le duration */ writer.Write((uint)this.Duration.TotalMilliseconds);
        }

        /// <inheritdoc />
        protected override void ReadPayload(BinaryReader reader) {
            /* uint8_t reserved */ reader.ReadByte();

            // HSBK
            ushort hue = reader.ReadUInt16();

            this.Hue = hue;

            ushort saturation = reader.ReadUInt16();

            this.Saturation = saturation;

            ushort brightness = reader.ReadUInt16();

            this.Brightness = brightness;

            ushort kelvin = reader.ReadUInt16();

            this.Kelvin = kelvin;

            uint duration = reader.ReadUInt32();

            this.Duration = TimeSpan.FromMilliseconds(duration);
        }
    }
}
