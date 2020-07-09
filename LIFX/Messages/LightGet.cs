using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Sent by a client to obtain the light state.
    /// </summary>
    internal class LightGet : LifxMessage {
        public const LifxMessageType TYPE = LifxMessageType.LightGet;

        public LightGet() : base(TYPE) {

        }
    }
}
