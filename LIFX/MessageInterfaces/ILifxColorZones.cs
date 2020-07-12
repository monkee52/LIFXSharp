using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    /// <summary>
    /// Common properties for set, and state messages for multizone lights
    /// </summary>
    public interface ILifxColorZones : ILifxColorZoneIndex {
        /// <value>The colors is 8/82 HSBK values and number of colors from this array will be applied sequentially from the <c>Index</c>'d zone on the device.</value>
        public IList<ILifxHsbkColor> Colors { get; }
    }
}
