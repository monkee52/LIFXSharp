using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class GetWifiFirmware : LifxMessage {
        public const LifxMessageType TYPE = LifxMessageType.GetWifiFirmware;

        public GetWifiFirmware() : base(TYPE) {

        }
    }
}
