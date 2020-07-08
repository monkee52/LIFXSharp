using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    internal class LifxAwaiter {
        public event LifxResponseReceivedEventHandler ResponseReceived;
        public event LifxExceptionEventHandler ExceptionGenerated;

        internal event EventHandler Finished;

        public Task Task { get; private set; }

        public LifxAwaiter(Task task) {
            this.Task = task;
        }

        internal void OnResponseReceived(object sender, IPEndPoint endPoint, LifxMessage message) {
            this.ResponseReceived?.Invoke(sender, new LifxResponseReceivedEventArgs(endPoint, message));
        }

        internal void OnExceptionGenerated(object sender, Exception exception) {
            this.ExceptionGenerated?.Invoke(sender, new LifxExceptionEventArgs(exception));
        }

        internal void OnFinished(object sender) {
            this.Finished?.Invoke(sender, null);
        }

        public static LifxAwaiter CreateSingleResponse(TaskCompletionSource<ILifxResponse<LifxMessage>> taskCompletionSource) {
            LifxAwaiter awaitingResponse = new LifxAwaiter(taskCompletionSource.Task);

            awaitingResponse.ResponseReceived += (object sender, LifxResponseReceivedEventArgs e) => {
                taskCompletionSource.SetResult(e);

                awaitingResponse.OnFinished(sender);
            };

            awaitingResponse.ExceptionGenerated += (object sender, LifxExceptionEventArgs e) => {
                taskCompletionSource.SetException(e.Exception);

                awaitingResponse.OnFinished(sender);
            };

            return awaitingResponse;
        }

        public static LifxAwaiter CreateMultipleResponse(TaskCompletionSource<IEnumerable<ILifxResponse<LifxMessage>>> taskCompletionSource) {
            LifxAwaiter awaitingResponse = new LifxAwaiter(taskCompletionSource.Task);

            IList<ILifxResponse<LifxMessage>> messages = new List<ILifxResponse<LifxMessage>>();

            awaitingResponse.ResponseReceived += (object sender, LifxResponseReceivedEventArgs e) => {
                messages.Add(e);
            };

            awaitingResponse.ExceptionGenerated += (object sender, LifxExceptionEventArgs e) => {
                if (e.Exception.GetType() == typeof(TimeoutException)) {
                    taskCompletionSource.SetResult(messages);
                } else {
                    taskCompletionSource.SetException(e.Exception);
                }

                awaitingResponse.OnFinished(sender);
            };

            return awaitingResponse;
        }
    }
}
