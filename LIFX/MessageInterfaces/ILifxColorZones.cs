﻿// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System.Collections.Generic;

namespace AydenIO.Lifx {
    /// <summary>
    /// Common properties for set, and state messages for multizone lights.
    /// </summary>
    public interface ILifxColorZones : ILifxColorZoneIndex {
        /// <summary>Gets the colors where colors is 8/82 HSBK values and number of colors from this array will be applied sequentially from the <c>Index</c>'d zone on the device.</summary>
        public IList<ILifxHsbkColor> Colors { get; }
    }
}
