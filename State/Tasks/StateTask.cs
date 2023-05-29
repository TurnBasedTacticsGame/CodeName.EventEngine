using System.Runtime.CompilerServices;

namespace CodeName.EventSystem.State.Tasks
{
    [AsyncMethodBuilder(typeof(StateTaskAsyncMethodBuilder))]
    public struct StateTask
    {
        public static StateTask CompletedTask => new(null);

        private readonly StateTaskCompletionSource source;

        public StateTask(StateTaskCompletionSource source)
        {
            this.source = source;
        }

        public bool IsCompleted => source?.IsCompleted ?? true;

        public StateTaskAwaiter GetAwaiter()
        {
            return new StateTaskAwaiter(source);
        }
    }

    [AsyncMethodBuilder(typeof(StateTaskAsyncMethodBuilder<>))]
    public struct StateTask<T>
    {
        public static StateTask<T> CompletedTask => new(null);

        private readonly StateTaskCompletionSource<T> source;

        public StateTask(StateTaskCompletionSource<T> source)
        {
            this.source = source;
        }

        public bool IsCompleted => source?.IsCompleted ?? true;

        public StateTaskAwaiter<T> GetAwaiter()
        {
            return new StateTaskAwaiter<T>(source);
        }

        public StateTask ToStateTask()
        {
            return new StateTask(source);
        }

        public static StateTask<T> FromResult(T result)
        {
            var completionSource = new StateTaskCompletionSource<T>();
            completionSource.Complete(result);

            return new StateTask<T>(completionSource);
        }
    }
}
