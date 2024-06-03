using System;
using System.Runtime.CompilerServices;

namespace CodeName.EventEngine.Tasks
{
    public struct StateTaskAsyncMethodBuilder
    {
        private readonly StateTaskCompletionSource source;

        public StateTaskAsyncMethodBuilder(StateTaskCompletionSource source)
        {
            this.source = source;
        }

        public StateTask Task => new(source);

        public static StateTaskAsyncMethodBuilder Create()
        {
            return new StateTaskAsyncMethodBuilder(new StateTaskCompletionSource());
        }

        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            stateMachine.MoveNext();
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine) {}

        public void SetResult()
        {
            source.Complete();
        }

        public void SetException(Exception exception)
        {
            source.Exception = exception;
            source.Complete();
        }

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            var capturedStateMachine = stateMachine;
            awaiter.OnCompleted(() =>
            {
                capturedStateMachine.MoveNext();
            });
        }

        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            AwaitOnCompleted(ref awaiter, ref stateMachine);
        }
    }

    public struct StateTaskAsyncMethodBuilder<T>
    {
        private readonly StateTaskCompletionSource<T> source;

        public StateTaskAsyncMethodBuilder(StateTaskCompletionSource<T> source)
        {
            this.source = source;
        }

        public StateTask<T> Task => new(source);

        public static StateTaskAsyncMethodBuilder<T> Create()
        {
            return new StateTaskAsyncMethodBuilder<T>(new StateTaskCompletionSource<T>());
        }

        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            stateMachine.MoveNext();
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine) {}

        public void SetResult(T result)
        {
            source.Complete(result);
        }

        public void SetException(Exception exception)
        {
            source.Exception = exception;
            source.Complete();
        }

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            var capturedStateMachine = stateMachine;
            awaiter.OnCompleted(() =>
            {
                capturedStateMachine.MoveNext();
            });
        }

        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            AwaitOnCompleted(ref awaiter, ref stateMachine);
        }
    }
}
