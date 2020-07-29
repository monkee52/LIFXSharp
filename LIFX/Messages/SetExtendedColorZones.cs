// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;
using System.Collections.Generic;
using System.IO;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// This messages lets you change all the zones on your device in one message.
    /// </summary>
    internal sealed class SetExtendedColorZones : LifxMessage, ILifxTransition, ILifxApplicationRequest, ILifxColorZones {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetExtendedColorZones"/> class.
        /// </summary>
        public SetExtendedColorZones() : base(LifxMessageType.SetExtendedColorZones) {
            this.Colors = new List<ILifxHsbkColor>();
        }

        /// <summary>Gets the current maximum number of zones in a LIFX MultiZone device.</summary>
        public static int MaxZoneCount => 82;

        /// <inheritdoc />
        public TimeSpan Duration { get; set; }

        /// <inheritdoc />
        public LifxApplicationRequest Apply { get; set; }

        /// <inheritdoc />
        public ushort Index { get; set; }

        /// <inheritdoc />
        public IList<ILifxHsbkColor> Colors { get; private set; }

        /// <inheritdoc />
        protected override void WritePayload(BinaryWriter writer) {
            /* uint32_t le duration */ writer.Write((uint)this.Duration.TotalMilliseconds);
            /* uint8_t apply */ writer.Write((byte)this.Apply);
            /* uint16_t le index */ writer.Write(this.Index);

            // HSBK color array
            int count = this.Colors.Count;

            /* uint8_t colors_count */ writer.Write((byte)count);

            ILifxHsbkColor defaultColor = new LifxHsbkColor();

            for (int i = 0; i < SetExtendedColorZones.MaxZoneCount; i++) {
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
            // Duration
            uint duration = reader.ReadUInt32();

            this.Duration = TimeSpan.FromMilliseconds(duration);

            // Apply
            byte apply = reader.ReadByte();

            this.Apply = (LifxApplicationRequest)apply;

            // Index
            ushort index = reader.ReadUInt16();

            this.Index = index;

            // HSBK color array
            byte count = reader.ReadByte();

            // Empty list
            this.Colors.Clear();

            for (int i = 0; i < SetExtendedColorZones.MaxZoneCount; i++) {
                // Read HSBK
                ushort hue = reader.ReadUInt16();
                ushort saturation = reader.ReadUInt16();
                ushort brightness = reader.ReadUInt16();
                ushort kelvin = reader.ReadUInt16();

                // Store
                if (i < count) {
                    ILifxHsbkColor color = new LifxHsbkColor {
                        Hue = hue,
                        Saturation = saturation,
                        Brightness = brightness,
                        Kelvin = kelvin,
                    };

                    this.Colors.Add(color);
                }
            }
        }
    }
}
