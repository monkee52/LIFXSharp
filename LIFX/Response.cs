// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System.Net;

namespace AydenIO.Lifx {
    /// <summary>
    /// Holds a grouping of an <see cref="IPEndPoint"/> and an untyped response.
    /// </summary>
    internal class Response : Response<LifxMessage> {
        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class.
        /// </summary>
        /// <param name="endPoint">The <see cref="IPEndPoint"/> that the message originated from.</param>
        /// <param name="message">The message.</param>
        public Response(IPEndPoint endPoint, LifxMessage message) : base(endPoint, message) {
            // Empty
        }
    }
}
