using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    public interface ILifxTime {
        /// <value>Current time</value>
        public DateTime Time { get; }
    }
}
