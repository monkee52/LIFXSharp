using System;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    /// <summary>
    /// Allows response(s) to be handled when received on the socket
    /// </summary>
    internal interface ILifxResponseAwaiter {
        /// <value>Gets the awaitable task</value>
        public Task Task { get; }

        /// <summary>
        /// Called whenever a response is received
        /// </summary>
        /// <param name="response">The response that was received</param>
        public void HandleResponse(LifxResponse response);

        /// <summary>
        /// Called whenever an exception is generated while waiting for a response
        /// </summary>
        /// <param name="exception">The exception that was generated</param>
        public void HandleException(Exception exception);
    }
}
