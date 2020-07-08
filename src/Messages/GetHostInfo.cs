using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class GetHostInfo : LifxMessage {
        public const LifxMessageType TYPE = LifxMessageType.GetHostInfo;

        public GetHostInfo() : base(TYPE) {

        }
    }
}
