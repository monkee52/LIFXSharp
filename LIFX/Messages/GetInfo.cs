// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Get run-time information.
    /// </summary>
    internal class GetInfo : LifxMessage {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetInfo"/> class.
        /// </summary>
        public GetInfo() : base(LifxMessageType.GetInfo) {
            // Empty
        }
    }
}
