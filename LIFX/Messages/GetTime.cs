using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class GetTime : LifxMessage {
        public const LifxMessageType TYPE = LifxMessageType.GetTime;

        public GetTime() : base(TYPE) {

        }
    }
}
