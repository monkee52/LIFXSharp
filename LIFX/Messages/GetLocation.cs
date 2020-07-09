using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Ask the bulb to return its location information.
    /// </summary>
    internal class GetLocation : LifxMessage {
        public const LifxMessageType TYPE = LifxMessageType.GetLocation;

        public GetLocation() : base(TYPE) {

        }
    }
}
