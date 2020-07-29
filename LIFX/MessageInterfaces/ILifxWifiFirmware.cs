// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;

namespace AydenIO.Lifx {
    /// <summary>
    /// A device's wifi firmware.
    /// </summary>
    public interface ILifxWifiFirmware {
        /// <summary>Gets the firmware build time.</summary>
        public DateTime Build { get; }

        /// <summary>Gets the firmware minor version.</summary>
        public ushort VersionMinor { get; }

        /// <summary>Gets the firmware major version.</summary>
        public ushort VersionMajor { get; }
    }
}
