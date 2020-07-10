using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    class LifxProduct : ILifxProduct {
        public string Name { get; set; }

        public bool SupportsColor { get; set; }
        public bool SupportsInfrared { get; set; }

        public bool IsMultizone { get; set; }
        public bool IsChain { get; set; }
        public bool IsMatrix { get; set; }

        public ushort MinKelvin { get; set; }
        public ushort MaxKelvin { get; set; }
    }
}
