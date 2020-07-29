// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System.IO;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Sent by a device to state its wifi info.
    /// </summary>
    internal class StateWifiInfo : LifxMessage, ILifxWifiInfo {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateWifiInfo"/> class.
        /// </summary>
        public StateWifiInfo() : base(LifxMessageType.StateWifiInfo) {
            // Empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateWifiInfo"/> class.
        /// </summary>
        /// <param name="wifiInfo">The <see cref="ILifxWifiInfo"/> to initialize this message from.</param>
        public StateWifiInfo(ILifxWifiInfo wifiInfo) : this() {
            this.Signal = wifiInfo.Signal;
            this.TransmittedBytes = wifiInfo.TransmittedBytes;
            this.ReceivedBytes = wifiInfo.ReceivedBytes;
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
            /* int16_t le reserved */ writer.Write((short)0);
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
