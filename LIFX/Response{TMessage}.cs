// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System.Net;

namespace AydenIO.Lifx {
    /// <summary>
    /// Holds a grouping of an <see cref="IPEndPoint"/> and a <typeparamref name="TMessage"/> message.
    /// </summary>
    /// <typeparam name="TMessage">The message type.</typeparam>
    internal class Response<TMessage> where TMessage : LifxMessage {
        /// <summary>
        /// Initializes a new instance of the <see cref="Response{TMessage}"/> class.
        /// </summary>
        /// <param name="endPoint">The <see cref="IPEndPoint"/> that the message originated from.</param>
        /// <param name="message">The message.</param>
        public Response(IPEndPoint endPoint, TMessage message) {
            this.EndPoint = endPoint;
            this.Message = message;
        }

        /// <summary>Gets the <see cref="IPEndPoint"/> that the message originated from.</summary>
        public IPEndPoint EndPoint { get; private set; }

        /// <summary>Gets the message.</summary>
        public TMessage Message { get; private set; }

        public static explicit operator Response<TMessage>(Response from) {
            return new Response<TMessage>(from.EndPoint, (TMessage)from.Message);
        }
    }
}
