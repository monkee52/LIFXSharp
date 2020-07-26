using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents a range of zones covered by a multizone state message.
    /// </summary>
    public interface ILifxColorZoneRange {
        /// <value>The index of the first zone.</value>
        public byte StartIndex { get; }

        /// <value>The index of the last zone.</value>
        public byte EndIndex { get; }
    }
}
