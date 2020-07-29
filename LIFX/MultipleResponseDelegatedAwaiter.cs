// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    /// <summary>
    /// An <see cref="IResponseAwaiter"/> that calls a delegate whenever a response is received, until the user cancels, or it times out.
    /// </summary>
    /// <typeparam name="T">The returned message type.</typeparam>
    internal sealed class MultipleResponseDelegatedAwaiter<T> : IResponseAwaiter where T : LifxMessage {
        private readonly TaskCompletionSource<bool> taskCompletionSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleResponseDelegatedAwaiter{T}"/> class.
        /// </summary>
        public MultipleResponseDelegatedAwaiter() {
            this.taskCompletionSource = new TaskCompletionSource<bool>(false);
        }

        /// <summary>An event that is invoked for every response received.</summary>
        public event Action<Response<T>> ResponseReceived;

        /// <inheritdoc />
        public Task Task => this.taskCompletionSource.Task;

        /// <inheritdoc />
        public void HandleResponse(Response<LifxMessage> response) {
            this.ResponseReceived?.Invoke((Response<T>)response);
        }

        /// <inheritdoc />
        public void HandleException(Exception e) {
            if (e is TimeoutException || e is OperationCanceledException) {
                this.taskCompletionSource.TrySetResult(true);
            } else {
                this.taskCompletionSource.SetException(e);
            }
        }
    }
}
