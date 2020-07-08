using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class GetVersion : LifxMessage {
        public const LifxMessageType TYPE = LifxMessageType.GetVersion;

        public GetVersion() : base(TYPE) {

        }
    }
}
