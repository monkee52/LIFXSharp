// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Ask the bulb to return its location information.
    /// </summary>
    internal sealed class GetLocation : LifxMessage {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetLocation"/> class.
        /// </summary>
        public GetLocation() : base(LifxMessageType.GetLocation) {
            // Empty
        }
    }
}
