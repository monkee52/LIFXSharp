using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Get run-time information.
    /// </summary>
    internal class GetInfo : LifxMessage {
        public const LifxMessageType TYPE = LifxMessageType.GetInfo;

        public GetInfo() : base(TYPE) {

        }
    }
}
