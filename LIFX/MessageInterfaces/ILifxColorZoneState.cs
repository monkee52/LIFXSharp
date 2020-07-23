using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents the state of a single zone on a multizone device
    /// </summary>
    public interface ILifxColorZoneState : ILifxColorZoneCount, ILifxColorZoneIndex, ILifxHsbkColor {
    }
}
