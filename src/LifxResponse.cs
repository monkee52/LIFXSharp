using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace AydenIO.Lifx {
    internal class LifxResponse<T> : ILifxResponse<T> where T : LifxMessage {
        public IPEndPoint EndPoint { get; private set; }

        public T Message { get; private set; }

        public static LifxResponse<T> From(ILifxResponse<LifxMessage> from) {
            return new LifxResponse<T>() {
                EndPoint = from.EndPoint,
                Message = (T)from.Message
            };
        }
    }
}
