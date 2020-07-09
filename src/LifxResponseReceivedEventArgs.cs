using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace AydenIO.Lifx {
    internal class LifxResponseReceivedEventArgs : EventArgs, ILifxResponse<LifxMessage> {
        public IPEndPoint EndPoint { get; private set; }
        public LifxMessage Message { get; private set; }

        public LifxResponseReceivedEventArgs(IPEndPoint endPoint, LifxMessage message) {
            this.EndPoint = endPoint;
            this.Message = message;
        }
    }

    internal delegate void LifxResponseReceivedEventHandler(object sender, LifxResponseReceivedEventArgs e);
}
