using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace CodeName.EventSystem.Tasks
{
    public struct StateTaskAwaiter : INotifyCompletion
    {
        private readonly StateTaskCompletionSource source;

        public StateTaskAwaiter(StateTaskCompletionSource source)
        {
            this.source = source;
        }

        public bool IsCompleted => source?.IsCompleted ?? true;

        public void GetResult()
        {
            if (source == null)
            {
                return;
            }

            if (source.Exception != null)
            {
                ExceptionDispatchInfo.Capture(source.Exception).Throw();
            }

            if (!IsCompleted)
            {
                throw new InvalidOperationException($"{typeof(StateTask).Name} has not completed yet.");
            }
        }

        public void OnCompleted(Action continuation)
        {
            if (IsCompleted)
            {
                continuation();

                return;
            }

            source.Continuation += continuation;
        }
    }

    public struct StateTaskAwaiter<T> : INotifyCompletion
    {
        private readonly StateTaskCompletionSource<T> source;

        public StateTaskAwaiter(StateTaskCompletionSource<T> source)
        {
            this.source = source;
        }

        public bool IsCompleted => source?.IsCompleted ?? true;

        public T GetResult()
        {
            if (source == null)
            {
                return default;
            }

            if (source.Exception != null)
            {
                ExceptionDispatchInfo.Capture(source.Exception).Throw();
            }

            if (!IsCompleted)
            {
                throw new InvalidOperationException($"{typeof(StateTask).Name} has not completed yet.");
            }

            return source.Result;
        }

        public void OnCompleted(Action continuation)
        {
            if (IsCompleted)
            {
                continuation();

                return;
            }

            source.Continuation += continuation;
        }
    }
}
