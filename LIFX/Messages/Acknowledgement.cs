// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Response to any message sent with _ack_required_ set to 1.
    /// </summary>
    internal sealed class Acknowledgement : LifxMessage {
        /// <summary>
        /// Initializes a new instance of the <see cref="Acknowledgement"/> class.
        /// </summary>
        public Acknowledgement() : base(LifxMessageType.Acknowledgement) {
            // Empty
        }
    }
}
