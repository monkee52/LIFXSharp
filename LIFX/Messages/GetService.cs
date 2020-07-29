// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Sent by a client to acquire responses from all devices on the local network.
    /// </summary>
    internal sealed class GetService : LifxMessage {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetService"/> class.
        /// </summary>
        public GetService() : base(LifxMessageType.GetService) {
            // Empty
        }
    }
}
