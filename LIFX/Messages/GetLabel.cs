// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Get device label.
    /// </summary>
    internal sealed class GetLabel : LifxMessage {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetLabel"/> class.
        /// </summary>
        public GetLabel() : base(LifxMessageType.GetLabel) {
            // Empty
        }
    }
}
