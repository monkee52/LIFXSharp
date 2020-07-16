using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    /// <summary>
    /// An <c>ILifxResponseAwaiter</c> that waits for all responses before the user cancels, or it times out
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class LifxMultipleResponseAwaiter<T> : ILifxResponseAwaiter where T : LifxMessage {
        private TaskCompletionSource<IReadOnlyCollection<LifxResponse<T>>> taskCompletionSource;

        private List<LifxResponse<T>> responses;

        /// <value>Gets the awaitable task</value>
        public Task<IReadOnlyCollection<LifxResponse<T>>> Task => this.taskCompletionSource.Task;

        /// <inheritdoc />
        Task ILifxResponseAwaiter.Task => this.taskCompletionSource.Task;

        /// <summary>
        /// Creates an awaiter that waits for all responses before the user cancels, or it times out
        /// </summary>
        public LifxMultipleResponseAwaiter() {
            this.taskCompletionSource = new TaskCompletionSource<IReadOnlyCollection<LifxResponse<T>>>();

            this.responses = new List<LifxResponse<T>>();
        }

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
