// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Get the hardware version.
    /// </summary>
    internal sealed class GetVersion : LifxMessage {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetVersion"/> class.
        /// </summary>
        public GetVersion() : base(LifxMessageType.GetVersion) {
            // Empty
        }
    }
}
