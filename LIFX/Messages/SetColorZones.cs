// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;
using System.IO;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// A message sent to a multizone device to set all zones between <see cref="StartIndex"/> and <see cref="EndIndex"/> to a color.
    /// </summary>
    internal sealed class SetColorZones : LifxMessage, ILifxColorZoneRange, ILifxHsbkColor, ILifxTransition, ILifxApplicationRequest {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetColorZones"/> class.
        /// </summary>
        public SetColorZones() : base(LifxMessageType.SetColorZones) {
            // Empty
        }

        /// <inheritdoc />
        public byte StartIndex { get; set; }

        /// <inheritdoc />
        public byte EndIndex { get; set; }

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
        public LifxApplicationRequest Apply { get; set; }

        /// <inheritdoc />
        public ILifxHsbkColor ToHsbk() {
            return this;
        }

        /// <inheritdoc />
        public void FromHsbk(ILifxHsbkColor color) {
            if (this != color && color is not null) {
                this.Hue = color.Hue;
                this.Saturation = color.Saturation;
                this.Brightness = color.Brightness;
                this.Kelvin = color.Kelvin;
            }
        }

        /// <inheritdoc />
        protected override void WritePayload(BinaryWriter writer) {
            /* uint8_t start_index */
            writer.Write(this.StartIndex);
            /* uint8_t end_index */
            writer.Write(this.EndIndex);

            // HSBK
            /* uint16_t le hue */
            writer.Write(this.Hue);
            /* uint16_t le saturation */
            writer.Write(this.Saturation);
            /* uint16_t le brightness */
            writer.Write(this.Brightness);
            /* uint16_t le kelvin */
            writer.Write(this.Kelvin);

            /* uint32_t le duration */
            writer.Write((uint)this.Duration.TotalMilliseconds);
            /* uint8_t apply */
            writer.Write((byte)this.Apply);
        }

        /// <inheritdoc />
        protected override void ReadPayload(BinaryReader reader) {
            byte startIndex = reader.ReadByte();

            this.StartIndex = startIndex;

            byte endIndex = reader.ReadByte();

            this.EndIndex = endIndex;

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

            byte apply = reader.ReadByte();

            this.Apply = (LifxApplicationRequest)apply;
        }
    }
}
