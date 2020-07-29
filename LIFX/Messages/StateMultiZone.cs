// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System.Collections.Generic;
using System.IO;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Sent by a multizone device to state up to 8 zones of color.
    /// </summary>
    internal sealed class StateMultiZone : LifxMessage, ILifxColorMultiZoneState {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateMultiZone"/> class.
        /// </summary>
        public StateMultiZone() : base(LifxMessageType.StateMultiZone) {
            this.Colors = new List<ILifxHsbkColor>();
        }

        /// <summary>Gets the maximum number of zones supported by this message.</summary>
        public static int MaxZoneCount => 8;

        /// <inheritdoc />
        public ushort ZoneCount { get; set; }

        /// <inheritdoc />
        public ushort Index { get; set; }

        /// <inheritdoc />
        public IList<ILifxHsbkColor> Colors { get; private set; }

        /// <inheritdoc />
        protected override void WritePayload(BinaryWriter writer) {
            /* uint8_t count */ writer.Write((byte)this.ZoneCount);
            /* uint8_t index */ writer.Write((byte)this.ZoneCount);

            int count = this.Colors.Count;

            ILifxHsbkColor defaultColor = new LifxHsbkColor();

            for (int i = 0; i < StateMultiZone.MaxZoneCount; i++) {
                ILifxHsbkColor color;

                if (i < count) {
                    color = this.Colors[i];
                } else {
                    color = defaultColor;
                }

                /* uint16_t le hue */ writer.Write(color.Hue);
                /* uint16_t le saturation */ writer.Write(color.Saturation);
                /* uint16_t le brightness */ writer.Write(color.Brightness);
                /* uint16_t le kelvin */ writer.Write(color.Kelvin);
            }
        }

        /// <inheritdoc />
        protected override void ReadPayload(BinaryReader reader) {
            byte zoneCount = reader.ReadByte();

            this.ZoneCount = zoneCount;

            byte index = reader.ReadByte();

            this.Index = index;

            this.Colors.Clear();

            for (int i = 0; i < StateMultiZone.MaxZoneCount; i++) {
                ILifxHsbkColor color = new LifxHsbkColor();

                // Read HSBK
                ushort hue = reader.ReadUInt16();

                color.Hue = hue;

                ushort saturation = reader.ReadUInt16();

                color.Saturation = saturation;

                ushort brightness = reader.ReadUInt16();

                color.Brightness = brightness;

                ushort kelvin = reader.ReadUInt16();

                color.Kelvin = kelvin;

                // Store
                this.Colors.Add(color);
            }
        }
    }
}
