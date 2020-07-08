using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Get device power level.
    /// </summary>
    internal class GetPower : LifxMessage {
        public const LifxMessageType TYPE = LifxMessageType.GetPower;

        public GetPower() : base(TYPE) {

        }
    }
}
