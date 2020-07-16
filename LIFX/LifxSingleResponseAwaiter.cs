using System;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    /// <summary>
    /// An <c>ILifxResponseAwaiter</c> that waits for a single response before completing
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class LifxSingleResponseAwaiter<T> : ILifxResponseAwaiter where T : LifxMessage {
        private TaskCompletionSource<LifxResponse<T>> taskCompletionSource;

        /// <value>Gets the awaitable task</value>
        public Task<LifxResponse<T>> Task => this.taskCompletionSource.Task;

        /// <inheritdoc />
        Task ILifxResponseAwaiter.Task => this.taskCompletionSource.Task;

        /// <summary>
        /// Creates an awaiter that waits for a single response
        /// </summary>
        public LifxSingleResponseAwaiter() {
            this.taskCompletionSource = new TaskCompletionSource<LifxResponse<T>>();
        }

        /// <inheritdoc />
        public void HandleResponse(LifxResponse response) {
            this.taskCompletionSource.SetResult((LifxResponse<T>)response);
        }

        /// <inheritdoc />
        public void HandleException(Exception e) {
            this.taskCompletionSource.SetException(e);
        }
    }
}
