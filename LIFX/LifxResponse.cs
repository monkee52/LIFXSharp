using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace AydenIO.Lifx {
    internal class LifxResponse<T> where T : LifxMessage {
        public IPEndPoint EndPoint { get; private set; }

        public T Message { get; private set; }

        public LifxResponse(IPEndPoint endPoint, T message) {
            this.EndPoint = endPoint;
            this.Message = message;
        }

        public static LifxResponse<T> From(LifxResponse<LifxMessage> from) {
            return new LifxResponse<T>(from.EndPoint, (T)from.Message);
        }
    }
}
