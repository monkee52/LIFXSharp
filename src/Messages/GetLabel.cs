using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class GetLabel : LifxMessage {
        public const LifxMessageType TYPE = LifxMessageType.GetLabel;

        public GetLabel() : base(TYPE) {

        }
    }
}
