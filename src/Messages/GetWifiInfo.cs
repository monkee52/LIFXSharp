using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class GetWifiInfo : LifxMessage {
        public const LifxMessageType TYPE = LifxMessageType.GetWifiInfo;

        public GetWifiInfo() : base(TYPE) {

        }
    }
}
