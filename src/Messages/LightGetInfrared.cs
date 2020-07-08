using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class LightGetInfrared : LifxMessage {
        public const LifxMessageType TYPE = LifxMessageType.LightGetInfrared;

        public LightGetInfrared() : base(TYPE) {

        }
    }
}
