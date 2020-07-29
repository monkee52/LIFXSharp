// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Sent to a device to get the current time.
    /// </summary>
    internal class GetTime : LifxMessage {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetTime"/> class.
        /// </summary>
        public GetTime() : base(LifxMessageType.GetTime) {
            // Empty
        }
    }
}
