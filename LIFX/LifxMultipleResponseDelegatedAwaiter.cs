using System;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    /// <summary>
    /// A <c>ILifxResponseAwaiter</c> that calls a delegate whenever a response is received, until the user cancels, or it times out
    /// </summary>
    /// <typeparam name="T">The returned message type</typeparam>
    internal class LifxMultipleResponseDelegatedAwaiter<T> : ILifxResponseAwaiter where T : LifxMessage {
        private readonly TaskCompletionSource<bool> taskCompletionSource;

        public event Action<LifxResponse<T>> ResponseReceived;

        public Task Task => this.taskCompletionSource.Task;

        /// <summary>
        /// Creates an awaiter that calls a delegate when responses are received before cancelled or timed out
        /// </summary>
        public LifxMultipleResponseDelegatedAwaiter() {
            this.taskCompletionSource = new TaskCompletionSource<bool>(false);
        }

        /// <inheritdoc />
        public void HandleResponse(LifxResponse response) {
            this.ResponseReceived?.Invoke((LifxResponse<T>)response);
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
