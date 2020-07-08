using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace AydenIO.Lifx {
    internal class LifxResponseReceivedEventArgs<T> : EventArgs, ILifxResponse<T> where T : LifxMessage {
        public IPEndPoint EndPoint { get; private set; }
        public T Message { get; private set; }

        public LifxResponseReceivedEventArgs(IPEndPoint endPoint, T message) {
            this.EndPoint = endPoint;
            this.Message = message;
        }
    }

    internal delegate void LifxResponseReceivedEventHandler<T>(object sender, LifxResponseReceivedEventArgs<T> e) where T : LifxMessage;

    internal class LifxResponseReceivedEventArgs : LifxResponseReceivedEventArgs<LifxMessage> {
        public LifxResponseReceivedEventArgs(IPEndPoint endPoint, LifxMessage message) : base(endPoint, message) {

        }
    }

    internal delegate void LifxResponseReceivedEventHandler(object sender, LifxResponseReceivedEventArgs e);
}
