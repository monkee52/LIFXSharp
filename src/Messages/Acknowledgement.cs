using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Response to any message sent with _ack_required_ set to 1
    /// </summary>
    internal class Acknowledgement : LifxMessage {
        public const LifxMessageType TYPE = LifxMessageType.Acknowledgement;

        public Acknowledgement() : base(TYPE) {

        }
    }
}
