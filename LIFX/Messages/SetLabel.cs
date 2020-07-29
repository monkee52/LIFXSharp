// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System.IO;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Sent to a device to change its label.
    /// </summary>
    internal sealed class SetLabel : LifxMessage, ILifxLabel {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetLabel"/> class.
        /// </summary>
        public SetLabel() : base(LifxMessageType.SetLabel) {
            // Empty
        }

        /// <inheritdoc />
        public string Label { get; set; }

        /// <inheritdoc />
        protected override void WritePayload(BinaryWriter writer) {
            /* uint8_t[32] label */ writer.Write(Utilities.StringToFixedBuffer(this.Label, 32));
        }

        /// <inheritdoc />
        protected override void ReadPayload(BinaryReader reader) {
            byte[] label = reader.ReadBytes(32);

            this.Label = Utilities.BufferToString(label);
        }
    }
}
