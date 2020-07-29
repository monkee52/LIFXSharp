// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    /// <summary>
    /// An <see cref="IResponseAwaiter"/> that waits for all responses before the user cancels, or it times out.
    /// </summary>
    /// <typeparam name="T">The response type.</typeparam>
    internal sealed class MultipleResponseAwaiter<T> : IResponseAwaiter where T : LifxMessage {
        private readonly TaskCompletionSource<IReadOnlyCollection<Response<T>>> taskCompletionSource;

        private readonly List<Response<T>> responses;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleResponseAwaiter{T}"/> class.
        /// </summary>
        public MultipleResponseAwaiter() {
            this.taskCompletionSource = new TaskCompletionSource<IReadOnlyCollection<Response<T>>>();

            this.responses = new List<Response<T>>();
        }

        /// <summary>Gets the awaitable task.</summary>
        public Task<IReadOnlyCollection<Response<T>>> Task => this.taskCompletionSource.Task;

        /// <inheritdoc />
        Task IResponseAwaiter.Task => this.taskCompletionSource.Task;

        /// <inheritdoc />
        public void HandleResponse(Response<LifxMessage> response) {
            this.responses.Add((Response<T>)response);
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
