using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    /// <summary>
    /// Common properties for <c>Messages.SetInfrared</c> and <c>Messages.StateInfrared</c>
    /// </summary>
    public interface ILifxInfrared {
        public ushort Level { get; set; }
    }
}
