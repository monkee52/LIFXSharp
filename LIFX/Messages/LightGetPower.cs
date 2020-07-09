using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Sent by a client to obtain the power level.
    /// </summary>
    internal class LightGetPower : LifxMessage {
        public const LifxMessageType TYPE = LifxMessageType.LightGetPower;

        public LightGetPower() : base(TYPE) {

        }
    }
}
