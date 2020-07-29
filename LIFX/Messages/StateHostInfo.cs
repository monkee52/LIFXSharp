// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System.IO;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Sent from a device stating its host info.
    /// </summary>
    internal sealed class StateHostInfo : LifxMessage, ILifxHostInfo {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateHostInfo"/> class.
        /// </summary>
        public StateHostInfo() : base(LifxMessageType.StateHostInfo) {
            // Empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateHostInfo"/> class.
        /// </summary>
        /// <param name="hostInfo">The <see cref="ILifxHostInfo"/> to initialize this message from.</param>
        public StateHostInfo(ILifxHostInfo hostInfo) : this() {
            this.Signal = hostInfo.Signal;
            this.TransmittedBytes = hostInfo.TransmittedBytes;
            this.ReceivedBytes = hostInfo.ReceivedBytes;
        }

        /// <inheritdoc />
        public float Signal { get; set; }

        /// <inheritdoc />
        public uint TransmittedBytes { get; set; }

        /// <inheritdoc />
        public uint ReceivedBytes { get; set; }

        /// <inheritdoc />
        public LifxSignalStrength GetSignalStrength() {
            return Utilities.GetSignalStrength(this.Signal);
        }

        /// <inheritdoc />
        protected override void WritePayload(BinaryWriter writer) {
            /* float32 le signal */ writer.Write(this.Signal);
            /* uint32_t le tx */ writer.Write(this.TransmittedBytes);
            /* uint32_t le rx */ writer.Write(this.ReceivedBytes);
            /* uint16_t le reserved */ writer.Write((ushort)0);
        }

        /// <inheritdoc />
        protected override void ReadPayload(BinaryReader reader) {
            float signal = reader.ReadSingle();

            this.Signal = signal;

            uint tx = reader.ReadUInt32();

            this.TransmittedBytes = tx;

            uint rx = reader.ReadUInt32();

            this.ReceivedBytes = rx;

            _ = reader.ReadInt16();
        }
    }
}
