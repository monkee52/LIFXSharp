// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents the count of zones in a multizone device.
    /// </summary>
    public interface ILifxColorZoneCount {
        /// <summary>Gets the number of zones the device has.</summary>
        public ushort ZoneCount { get; }
    }
}
