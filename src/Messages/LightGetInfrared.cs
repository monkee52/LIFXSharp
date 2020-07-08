using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Gets the current maximum power level of the Infrared channel.
    /// </summary>
    internal class LightGetInfrared : LifxMessage {
        public const LifxMessageType TYPE = LifxMessageType.LightGetInfrared;

        public LightGetInfrared() : base(TYPE) {

        }
    }
}
