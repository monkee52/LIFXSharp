using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents the count of zones in a multizone device
    /// </summary>
    public interface ILifxColorZoneCount {
        /// <value>The number of zones the device has</value>
        public ushort ZoneCount { get; set; }
    }
}
