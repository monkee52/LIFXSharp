using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class GetAccessPoints : LifxMessage {
        public const LifxMessageType TYPE = LifxMessageType.GetAccessPoints;

        public GetAccessPoints() : base(TYPE) {

        }
    }
}
