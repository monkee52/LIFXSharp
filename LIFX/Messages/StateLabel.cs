// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System.IO;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Sent by a device to state its current label.
    /// </summary>
    internal class StateLabel : LifxMessage, ILifxLabel {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateLabel"/> class.
        /// </summary>
        public StateLabel() : base(LifxMessageType.StateLabel) {
            // Empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateLabel"/> class.
        /// </summary>
        /// <param name="label">The string label to initialize this message from.</param>
        public StateLabel(string label) : this() {
            this.Label = label;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateLabel"/> class.
        /// </summary>
        /// <param name="label">The <see cref="ILifxLabel"/> to initialize this message from.</param>
        public StateLabel(ILifxLabel label) : this() {
            this.Label = label.Label;
        }

        /// <inheritdoc />
        public string Label { get; set; }

        /// <inheritdoc />
        protected override void WritePayload(BinaryWriter writer) {
            writer.Write(Utilities.StringToFixedBuffer(this.Label, 32));
        }

        /// <inheritdoc />
        protected override void ReadPayload(BinaryReader reader) {
            byte[] label = reader.ReadBytes(32);

            this.Label = Utilities.BufferToString(label);
        }
    }
}
