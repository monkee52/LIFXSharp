using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class GetService : LifxMessage {
        public const LifxMessageType TYPE = LifxMessageType.GetService;

        public GetService() : base(TYPE) {

        }
    }
}
