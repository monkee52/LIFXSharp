using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class GetTags : LifxMessage {
        public const LifxMessageType TYPE = LifxMessageType.GetTags;

        public GetTags() : base(TYPE) {

        }
    }
}
