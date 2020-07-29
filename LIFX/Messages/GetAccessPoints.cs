// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// A message sent to a device to retrieve the list of access points the device can se.
    /// </summary>
    internal sealed class GetAccessPoints : LifxMessage {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetAccessPoints"/> class.
        /// </summary>
        public GetAccessPoints() : base(LifxMessageType.GetAccessPoints) {
            // Empty
        }
    }
}
