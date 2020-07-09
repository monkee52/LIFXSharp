using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    internal enum LifxeResponseFlags {
        None = 0,
        ResponseRequired = 1,
        AcknowledgementRequired = 2
    }
}
