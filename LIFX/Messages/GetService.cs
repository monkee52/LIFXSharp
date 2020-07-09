using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Sent by a client to acquire responses from all devices on the local network.
    /// </summary>
    internal class GetService : LifxMessage {
        public const LifxMessageType TYPE = LifxMessageType.GetService;

        public GetService() : base(TYPE) {

        }
    }
}
