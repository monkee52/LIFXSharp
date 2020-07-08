using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace AydenIO.Lifx {
    public interface ILifxResponse<T> {
        public IPEndPoint EndPoint { get; }
        public T Message { get; }
    }
}
