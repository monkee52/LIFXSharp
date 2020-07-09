using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    public interface ILifxDeviceFeatures {
        public string Name { get; }

        public bool SupportsColor { get; }
        public bool SupportsTemperature { get; }
        public bool SupportsInfrared { get; }

        public bool IsMultizone { get; }
        public bool IsChain { get; }

        public ushort MinKelvin { get; }
        public ushort MaxKelvin { get; }
    }
}
