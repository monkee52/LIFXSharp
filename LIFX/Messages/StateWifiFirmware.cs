// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;
using System.IO;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Sent by a device to state its wifi firmware.
    /// </summary>
    internal class StateWifiFirmware : LifxMessage, ILifxWifiFirmware {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateWifiFirmware"/> class.
        /// </summary>
        public StateWifiFirmware() : base(LifxMessageType.StateWifiFirmware) {
            // Empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateWifiFirmware"/> class.
        /// </summary>
        /// <param name="wifiFirmware">The <see cref="ILifxWifiFirmware"/> to initialize this message from.</param>
        public StateWifiFirmware(ILifxWifiFirmware wifiFirmware) : this() {
            this.Build = wifiFirmware.Build;
            this.VersionMinor = wifiFirmware.VersionMinor;
            this.VersionMajor = wifiFirmware.VersionMajor;
        }

        /// <inheritdoc />
        public DateTime Build { get; set; }

        /// <inheritdoc />
        public ushort VersionMinor { get; set; }

        /// <inheritdoc />
        public ushort VersionMajor { get; set; }

        /// <inheritdoc />
        protected override void WritePayload(BinaryWriter writer) {
            /* uint64_t le build */ writer.Write(Utilities.DateTimeToNanoseconds(this.Build));
            /* uint64_t le reserved */ writer.Write(0uL);
            /* uint16_t le minor */ writer.Write(this.VersionMinor);
            /* uint16_t le major */ writer.Write(this.VersionMajor);
        }

        /// <inheritdoc />
        protected override void ReadPayload(BinaryReader reader) {
            // Build
            ulong build = reader.ReadUInt64();

            this.Build = Utilities.NanosecondsToDateTime(build);

            // Reserved
            _ = reader.ReadUInt64();

            // Minor version
            ushort minor = reader.ReadUInt16();

            this.VersionMinor = minor;

            // Major version
            ushort major = reader.ReadUInt16();

            this.VersionMajor = major;
        }
    }
}
