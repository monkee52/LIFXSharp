using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Gets Host MCU firmware information.
    /// </summary>
    internal class GetHostFirmware : LifxMessage {
        public const LifxMessageType TYPE = LifxMessageType.GetHostFirmware;

        public GetHostFirmware() : base(TYPE) {

        }
    }
}
