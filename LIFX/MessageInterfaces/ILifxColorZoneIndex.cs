using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    public interface ILifxColorZoneIndex {
        /// <value>The index field is the zone we start applying the colors from. The first zone is 0.</value>
        public ushort Index { get; set; }
    }
}
