using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Sent by a device to provide the current power level.
    /// </summary>
    internal class LightStatePower : LifxMessage, ILifxPower {
        public const LifxMessageType TYPE = LifxMessageType.LightStatePower;

        public LightStatePower() : base(TYPE) {

        }

        public LightStatePower(bool poweredOn) : this() {
            this.PoweredOn = poweredOn;
        }

        public LightStatePower(ILifxPower power) : this() {
            this.PoweredOn = power.PoweredOn;
        }

        public bool PoweredOn { get; set; }

        protected override void WritePayload(BinaryWriter writer) {
            /* uint16_t le level */ writer.Write((ushort)(this.PoweredOn ? 65535 : 0));
        }

        protected override void ReadPayload(BinaryReader reader) {
            ushort poweredOn = reader.ReadUInt16();

            this.PoweredOn = poweredOn >= 32768;
        }
    }
}
