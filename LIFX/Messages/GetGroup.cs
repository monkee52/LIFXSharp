using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Ask the bulb to return its group membership information.
    /// </summary>
    internal class GetGroup : LifxMessage {
        public const LifxMessageType TYPE = LifxMessageType.GetGroup;

        public GetGroup() : base(TYPE) {

        }
    }
}
