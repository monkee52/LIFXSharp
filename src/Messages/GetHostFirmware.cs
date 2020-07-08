using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class GetHostFirmware : LifxMessage {
        public const LifxMessageType TYPE = LifxMessageType.GetHostFirmware;

        public GetHostFirmware() : base(TYPE) {

        }
    }
}
