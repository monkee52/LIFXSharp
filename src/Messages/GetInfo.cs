using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class GetInfo : LifxMessage {
        public const LifxMessageType TYPE = LifxMessageType.GetInfo;

        public GetInfo() : base(TYPE) {

        }
    }
}
