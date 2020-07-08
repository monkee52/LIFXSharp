using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class LightGet : LifxMessage {
        public const LifxMessageType TYPE = LifxMessageType.LightGet;

        public LightGet() : base(TYPE) {

        }
    }
}
