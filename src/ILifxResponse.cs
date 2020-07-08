using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace AydenIO.Lifx {
    internal interface ILifxResponse<T> where T : LifxMessage {
        public IPEndPoint EndPoint { get; }
        public T Message { get; }
    }
}
