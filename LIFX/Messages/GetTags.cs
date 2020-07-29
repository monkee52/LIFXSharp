// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Sent to a device to retrieve its tags.
    /// </summary>
    internal sealed class GetTags : LifxMessage {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetTags"/> class.
        /// </summary>
        public GetTags() : base(LifxMessageType.GetTags) {
            // Empty
        }
    }
}
