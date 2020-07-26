using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class StateExtendedColorZones : LifxMessage, ILifxColorMultiZoneState {
        public const LifxMessageType TYPE = LifxMessageType.StateExtendedColorZones;

        /// <value>The current maximum number of zones in a LIFX MultiZone device</value>
        public static int MaxZoneCount => 82;

        public StateExtendedColorZones() : base(TYPE) {
            this.Colors = new List<ILifxHsbkColor>();
        }

        public StateExtendedColorZones(ILifxColorMultiZoneState state) {
            this.Index = state.Index;
            this.ZoneCount = state.ZoneCount;

            foreach (ILifxHsbkColor color in state.Colors) {
                this.Colors.Add(color);
            }
        }

        public ushort ZoneCount { get; set; }

        public ushort Index { get; set; }

        public IList<ILifxHsbkColor> Colors { get; private set; }

        protected override void WritePayload(BinaryWriter writer) {
            /* uint16_t le count */ writer.Write(this.ZoneCount);
            /* uint16_t le index */ writer.Write(this.Index);

            // HSBK color array
            int count = this.Colors.Count;

            /* uint8_t colors_count */ writer.Write((byte)count);

            ILifxHsbkColor defaultColor = new LifxHsbkColor();

            for (int i = 0; i < StateExtendedColorZones.MaxZoneCount; i++) {
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

        protected override void ReadPayload(BinaryReader reader) {
            // ZoneCount
            ushort zoneCount = reader.ReadUInt16();

            this.ZoneCount = zoneCount;

            // Index
            ushort index = reader.ReadUInt16();

            this.Index = index;

            // HSBK color array
            byte count = reader.ReadByte();

            // Empty list
            this.Colors.Clear();

            for (int i = 0; i < StateExtendedColorZones.MaxZoneCount; i++) {
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
                        Kelvin = kelvin
                    };

                    this.Colors.Add(color);
                }
            }
        }
    }
}
