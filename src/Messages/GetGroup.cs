using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class GetGroup : LifxMessage {
        public const LifxMessageType TYPE = LifxMessageType.GetGroup;

        public GetGroup() : base(TYPE) {

        }
    }
}
