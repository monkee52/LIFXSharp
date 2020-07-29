// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// This message will ask the device to return a StateExtendedColorZones containing all of it's colors.
    /// </summary>
    internal class GetExtendedColorZones : LifxMessage {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetExtendedColorZones"/> class.
        /// </summary>
        public GetExtendedColorZones() : base(LifxMessageType.GetExtendedColorZones) {
            // Empty
        }
    }
}
