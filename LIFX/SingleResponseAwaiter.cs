// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    /// <summary>
    /// An <see cref="IResponseAwaiter"/> that waits for a single response before completing.
    /// </summary>
    /// <typeparam name="TMessage">The message type.</typeparam>
    internal sealed class SingleResponseAwaiter<TMessage> : IResponseAwaiter where TMessage : LifxMessage {
        private readonly TaskCompletionSource<Response<TMessage>> taskCompletionSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleResponseAwaiter{TMessage}"/> class.
        /// </summary>
        public SingleResponseAwaiter() {
            this.taskCompletionSource = new TaskCompletionSource<Response<TMessage>>();
        }

        /// <summary>Gets the awaitable task.</summary>
        public Task<Response<TMessage>> Task => this.taskCompletionSource.Task;

        /// <inheritdoc />
        Task IResponseAwaiter.Task => this.taskCompletionSource.Task;

        /// <inheritdoc />
        public void HandleResponse(Response<LifxMessage> response) {
            this.taskCompletionSource.SetResult((Response<TMessage>)response);
        }

        /// <inheritdoc />
        public void HandleException(Exception e) {
            this.taskCompletionSource.SetException(e);
        }
    }
}
