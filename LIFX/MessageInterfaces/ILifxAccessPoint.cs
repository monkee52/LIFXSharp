using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    public interface ILifxAccessPoint : ILifxWifiInterface {
        public string Ssid { get; }

        public LifxSecurityProtocol SecurityProtocol { get; }

        public ushort Strength { get; }

        public ushort Channel { get; }
    }
}
