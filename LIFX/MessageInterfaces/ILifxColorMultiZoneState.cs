using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents multiple zones on a MultiZone device
    /// </summary>
    public interface ILifxColorMultiZoneState : ILifxColorZones, ILifxColorZoneIndex, ILifxColorZoneCount {
    }
}
