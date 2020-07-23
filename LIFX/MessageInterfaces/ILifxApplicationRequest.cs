using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    /// <summary>
    /// This type allows you to provide hints to the device about how the changes you make should be performed.
    /// </summary>
    public interface ILifxApplicationRequest {
        /// <value>How the change should be performed.</value>
        public LifxApplicationRequest Apply { get; set; }
    }
}
