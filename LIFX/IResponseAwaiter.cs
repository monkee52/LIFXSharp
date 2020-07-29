// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    /// <summary>
    /// Allows response(s) to be handled when received on the socket.
    /// </summary>
    internal interface IResponseAwaiter {
        /// <summary>Gets the awaitable task.</summary>
        public Task Task { get; }

        /// <summary>
        /// Called whenever a response is received.
        /// </summary>
        /// <param name="response">The response that was received.</param>
        public void HandleResponse(Response response);

        /// <summary>
        /// Called whenever an exception is generated while waiting for a response.
        /// </summary>
        /// <param name="exception">The exception that was generated.</param>
        public void HandleException(Exception exception);
    }
}
