// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System.IO;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Sent by a device to provide the current light state.
    /// </summary>
    internal sealed class LightState : LifxMessage, ILifxLightState {
        /// <summary>
        /// Initializes a new instance of the <see cref="LightState"/> class.
        /// </summary>
        public LightState() : base(LifxMessageType.LightState) {
            // Empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LightState"/> class.
        /// </summary>
        /// <param name="lightState">The <see cref="ILifxLightState"/> to initialize this message from.</param>
        public LightState(ILifxLightState lightState) : this() {
            this.FromHsbk(lightState);

            this.PoweredOn = lightState.PoweredOn;
            this.Label = lightState.Label;
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
        public bool PoweredOn { get; set; }

        /// <inheritdoc />
        public string Label { get; set; }

        /// <inheritdoc />
        public ILifxHsbkColor ToHsbk() {
            return this;
        }

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
        protected override void WritePayload(BinaryWriter writer) {
            // HSBK
            /* uint16_t le hue */ writer.Write(this.Hue);
            /* uint16_t le saturation */ writer.Write(this.Saturation);
            /* uint16_t le brightness */ writer.Write(this.Brightness);
            /* uint16_t le kelvin */ writer.Write(this.Kelvin);

            /* int16_t le reserved */ writer.Write((short)0);
            /* uint16_t le power */ writer.Write((ushort)(this.PoweredOn ? 65535 : 0));

            // Label
            /* uint8_t[32] label */ writer.Write(Utilities.StringToFixedBuffer(this.Label, 32));

            // Reserved
            /* uint64_t le reserved */ writer.Write(0uL);
        }

        /// <inheritdoc />
        protected override void ReadPayload(BinaryReader reader) {
            // HSBK
            ushort hue = reader.ReadUInt16();

            this.Hue = hue;

            ushort saturation = reader.ReadUInt16();

            this.Saturation = saturation;

            ushort brightness = reader.ReadUInt16();

            this.Brightness = brightness;

            ushort kelvin = reader.ReadUInt16();

            this.Kelvin = kelvin;

            /* int16_t le reserved */ reader.ReadInt16();

            ushort power = reader.ReadUInt16();

            this.PoweredOn = power >= 32768;

            byte[] label = reader.ReadBytes(32);

            this.Label = Utilities.BufferToString(label);

            /* uint64_t le reserved */ reader.ReadUInt64();
        }
    }
}
