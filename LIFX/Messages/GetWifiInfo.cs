using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Get Wifi subsystem information.
    /// </summary>
    internal class GetWifiInfo : LifxMessage {
        public const LifxMessageType TYPE = LifxMessageType.GetWifiInfo;

        public GetWifiInfo() : base(TYPE) {

        }
    }
}
