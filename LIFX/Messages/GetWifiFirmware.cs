using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Get Wifi subsystem firmware.
    /// </summary>
    internal class GetWifiFirmware : LifxMessage {
        public const LifxMessageType TYPE = LifxMessageType.GetWifiFirmware;

        public GetWifiFirmware() : base(TYPE) {

        }
    }
}
