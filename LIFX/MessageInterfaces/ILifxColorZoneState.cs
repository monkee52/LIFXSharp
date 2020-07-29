// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents the state of a single zone on a multizone device.
    /// </summary>
    public interface ILifxColorZoneState : ILifxColorZoneCount, ILifxColorZoneIndex, ILifxHsbkColor {
    }
}
