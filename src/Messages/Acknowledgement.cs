using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class Acknowledgement : LifxMessage {
        public const LifxMessageType TYPE = LifxMessageType.Acknowledgement;

        public Acknowledgement() : base(TYPE) {

        }
    }
}
