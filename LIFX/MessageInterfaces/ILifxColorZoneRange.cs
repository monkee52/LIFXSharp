using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    public interface ILifxColorZoneRange {
        public byte StartIndex { get; set; }
        public byte EndIndex { get; set; }
    }
}
