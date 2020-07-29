// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;
using System.IO;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Sent from a device stating its host firmware.
    /// </summary>
    internal sealed class StateHostFirmware : LifxMessage, ILifxHostFirmware {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateHostFirmware"/> class.
        /// </summary>
        public StateHostFirmware() : base(LifxMessageType.StateHostFirmware) {
            // Empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateHostFirmware"/> class.
        /// </summary>
        /// <param name="hostFirmware">The <see cref="ILifxHostFirmware"/> to initialize this message from.</param>
        public StateHostFirmware(ILifxHostFirmware hostFirmware) : this() {
            this.Build = hostFirmware.Build;
            this.VersionMinor = hostFirmware.VersionMinor;
            this.VersionMajor = hostFirmware.VersionMajor;
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
            ulong build = reader.ReadUInt64();

            this.Build = Utilities.NanosecondsToDateTime(build);

            _ = reader.ReadUInt64();

            ushort minor = reader.ReadUInt16();

            this.VersionMinor = minor;

            ushort major = reader.ReadUInt16();

            this.VersionMajor = major;
        }
    }
}
