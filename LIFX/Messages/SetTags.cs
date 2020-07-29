// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Sent to a device to set its tags.
    /// </summary>
    internal sealed class SetTags : LifxMessage, ILifxTagId {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetTags"/> class.
        /// </summary>
        public SetTags() : base(LifxMessageType.SetTags) {
            // Empty
        }

        /// <inheritdoc />
        public ulong TagId { get; set; }
    }
}
