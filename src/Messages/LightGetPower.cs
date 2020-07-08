using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class LightGetPower : LifxMessage {
        public const LifxMessageType TYPE = LifxMessageType.LightGetPower;

        public LightGetPower() : base(TYPE) {

        }
    }
}
