using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    public interface ILifxService {
        public LifxService Service { get; set; }
        public uint Port { get; set; }
    }
}
