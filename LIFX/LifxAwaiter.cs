using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    internal enum LifxAwaiterType {
        Multiple,
        Single,
        Delegated
    }

    internal class LifxAwaiter {
        private readonly IList<LifxResponse<LifxMessage>> messages;

        public Task Task { get; private set; }

        public LifxAwaiterType Type { get; private set; }

        private readonly Action<LifxResponse<LifxMessage>> responseHandler;
        private readonly Action finishHandler;

        private readonly Action<Exception> setException;

        public LifxAwaiter(TaskCompletionSource<IEnumerable<LifxResponse<LifxMessage>>> tcs) {
            this.Task = tcs.Task;

            this.Type = LifxAwaiterType.Multiple;

            this.messages = new List<LifxResponse<LifxMessage>>();

            this.responseHandler = this.messages.Add;
            this.finishHandler = () => tcs.SetResult(this.messages);

            this.setException = tcs.SetException;
        }

        public LifxAwaiter(TaskCompletionSource<LifxResponse<LifxMessage>> tcs) {
            this.Task = tcs.Task;

            this.Type = LifxAwaiterType.Single;

            this.messages = null;

            this.responseHandler = tcs.SetResult;
            this.finishHandler = null;

            this.setException = tcs.SetException;
        }

        public LifxAwaiter(Action<LifxResponse<LifxMessage>> handler) {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>(false);

            this.Task = tcs.Task;

            this.Type = LifxAwaiterType.Delegated;

            this.messages = null;

            this.responseHandler = handler;
            this.finishHandler = () => tcs.SetResult(true);

            this.setException = tcs.SetException;
        }

        internal void HandleResponse(IPEndPoint endPoint, LifxMessage message) {
            LifxResponse<LifxMessage> response = new LifxResponse<LifxMessage>(endPoint, message);

            this.responseHandler?.Invoke(response);
        }

        internal void HandleException(Exception exception) {
            if ((this.Type == LifxAwaiterType.Multiple || this.Type == LifxAwaiterType.Delegated) && (exception is TimeoutException || exception is OperationCanceledException)) {
                this.finishHandler?.Invoke();
            } else {
                this.setException(exception);
            }
        }
    }
}
