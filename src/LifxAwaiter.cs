using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    internal class LifxAwaiter {
        private readonly IList<LifxResponse<LifxMessage>> messages;

        public Task Task { get; private set; }

        private readonly Action<IEnumerable<LifxResponse<LifxMessage>>> setResultMultiple;
        private readonly Action<LifxResponse<LifxMessage>> setResultSingle;

        private readonly Action<Exception> setException;

        public LifxAwaiter(TaskCompletionSource<IEnumerable<LifxResponse<LifxMessage>>> tcs) {
            this.Task = tcs.Task;

            this.setResultMultiple = tcs.SetResult;
            this.setException = tcs.SetException;

            this.messages = new List<LifxResponse<LifxMessage>>();
        }

        public LifxAwaiter(TaskCompletionSource<LifxResponse<LifxMessage>> tcs) {
            this.Task = tcs.Task;

            this.setResultSingle = tcs.SetResult;
            this.setException = tcs.SetException;

            this.messages = null;
        }

        internal void HandleResponse(IPEndPoint endPoint, LifxMessage message) {
            LifxResponse<LifxMessage> response = new LifxResponse<LifxMessage>(endPoint, message);

            if (this.messages != null) {
                this.messages.Add(response);
            } else {
                this.setResultSingle(response);
            }
        }

        internal void HandleException(Exception exception) {
            if (this.messages != null && exception is TimeoutException) {
                this.setResultMultiple(this.messages);
            } else {
                this.setException(exception);
            }
        }
    }
}
