// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Ask the bulb to return its group membership information.
    /// </summary>
    internal class GetGroup : LifxMessage {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetGroup"/> class.
        /// </summary>
        public GetGroup() : base(LifxMessageType.GetGroup) {
            // Empty
        }
    }
}
