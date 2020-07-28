// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    /// <summary>
    /// An <see cref="ILifxResponseAwaiter"/> that waits for all responses before the user cancels, or it times out.
    /// </summary>
    /// <typeparam name="T">The response type.</typeparam>
    internal class LifxMultipleResponseAwaiter<T> : ILifxResponseAwaiter where T : LifxMessage {
        private readonly TaskCompletionSource<IReadOnlyCollection<LifxResponse<T>>> taskCompletionSource;

        private readonly List<LifxResponse<T>> responses;

        /// <summary>
        /// Initializes a new instance of the <see cref="LifxMultipleResponseAwaiter{T}"/> class.
        /// </summary>
        public LifxMultipleResponseAwaiter() {
            this.taskCompletionSource = new TaskCompletionSource<IReadOnlyCollection<LifxResponse<T>>>();

            this.responses = new List<LifxResponse<T>>();
        }

        /// <summary>Gets the awaitable task.</summary>
        public Task<IReadOnlyCollection<LifxResponse<T>>> Task => this.taskCompletionSource.Task;

        /// <inheritdoc />
        Task ILifxResponseAwaiter.Task => this.taskCompletionSource.Task;

        /// <inheritdoc />
        public void HandleResponse(LifxResponse response) {
            this.responses.Add((LifxResponse<T>)response);
        }

        /// <inheritdoc />
        public void HandleException(Exception e) {
            if (e is TimeoutException || e is OperationCanceledException) {
                this.taskCompletionSource.SetResult(this.responses.AsReadOnly());
            } else {
                this.taskCompletionSource.SetException(e);
            }
        }
    }
}
