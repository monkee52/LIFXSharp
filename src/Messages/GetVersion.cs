using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Get the hardware version.
    /// </summary>
    internal class GetVersion : LifxMessage {
        public const LifxMessageType TYPE = LifxMessageType.GetVersion;

        public GetVersion() : base(TYPE) {

        }
    }
}
