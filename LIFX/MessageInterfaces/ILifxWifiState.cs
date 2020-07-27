using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace AydenIO.Lifx {
    public interface ILifxWifiState : ILifxWifiInterface {
        public LifxWifiStatus Status { get; }

        public IPAddress IPAddressV4 { get; }

        public IPAddress IPAddressV6 { get; }
    }
}
