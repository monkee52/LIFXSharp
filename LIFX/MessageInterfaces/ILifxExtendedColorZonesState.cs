using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    public interface ILifxExtendedColorZonesState : ILifxExtendedColorZones {
        public ushort ZoneCount { get; set; }
    }
}
