using System;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    /// <summary>
    /// A <c>ILifxResponseAwaiter</c> that calls a delegate whenever a response is received, until the user cancels, or it times out
    /// </summary>
    /// <typeparam name="T">The returned message type</typeparam>
    internal class LifxMultipleResponseDelegatedAwaiter<T> : ILifxResponseAwaiter where T : LifxMessage {
        private TaskCompletionSource<bool> taskCompletionSource;

        private Action<LifxResponse<T>> responseHandler;

        public Task Task => this.taskCompletionSource.Task;

        /// <summary>
        /// Creates an awaiter that calls a delegate when responses are received before cancelled or timed out
        /// </summary>
        /// <param name="responseHandler">The delegate to invoke for each response</param>
        public LifxMultipleResponseDelegatedAwaiter(Action<LifxResponse<T>> responseHandler) {
            this.taskCompletionSource = new TaskCompletionSource<bool>(false);

            this.responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public void HandleResponse(LifxResponse response) {
            this.responseHandler?.Invoke((LifxResponse<T>)response);
        }

        /// <inheritdoc />
        public void HandleException(Exception e) {
            if (e is TimeoutException || e is OperationCanceledException) {
                this.taskCompletionSource.SetResult(true);
            } else {
                this.taskCompletionSource.SetException(e);
            }
        }
    }
}
