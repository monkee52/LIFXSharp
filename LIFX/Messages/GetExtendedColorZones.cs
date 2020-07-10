using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// This message will ask the device to return a StateExtendedColorZones containing all of it's colors.
    /// </summary>
    internal class GetExtendedColorZones : LifxMessage {
        public const LifxMessageType TYPE = LifxMessageType.GetExtendedColorZones;

        public GetExtendedColorZones() : base(TYPE) {

        }
    }
}
