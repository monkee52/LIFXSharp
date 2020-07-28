// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    /// <summary>
    /// An <see cref="ILifxResponseAwaiter"/> that waits for a single response before completing.
    /// </summary>
    /// <typeparam name="TMessage">The message type.</typeparam>
    internal class LifxSingleResponseAwaiter<TMessage> : ILifxResponseAwaiter where TMessage : LifxMessage {
        private readonly TaskCompletionSource<LifxResponse<TMessage>> taskCompletionSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="LifxSingleResponseAwaiter{TMessage}"/> class.
        /// </summary>
        public LifxSingleResponseAwaiter() {
            this.taskCompletionSource = new TaskCompletionSource<LifxResponse<TMessage>>();
        }

        /// <summary>Gets the awaitable task.</summary>
        public Task<LifxResponse<TMessage>> Task => this.taskCompletionSource.Task;

        /// <inheritdoc />
        Task ILifxResponseAwaiter.Task => this.taskCompletionSource.Task;

        /// <inheritdoc />
        public void HandleResponse(LifxResponse response) {
            this.taskCompletionSource.SetResult((LifxResponse<TMessage>)response);
        }

        /// <inheritdoc />
        public void HandleException(Exception e) {
            this.taskCompletionSource.SetException(e);
        }
    }
}
