// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;

namespace AydenIO.Lifx {
    /// <summary>
    /// Device's host firmware.
    /// </summary>
    public interface ILifxHostFirmware {
        /// <summary>Gets the firmware build time.</summary>
        public DateTime Build { get; }

        /// <summary>Gets the firmware minor version.</summary>
        public ushort VersionMinor { get; }

        /// <summary>Gets the firmware major version.</summary>
        public ushort VersionMajor { get; }
    }
}
