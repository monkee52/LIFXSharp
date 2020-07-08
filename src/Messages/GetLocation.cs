using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class GetLocation : LifxMessage {
        public const LifxMessageType TYPE = LifxMessageType.GetLocation;

        public GetLocation() : base(TYPE) {

        }
    }
}
