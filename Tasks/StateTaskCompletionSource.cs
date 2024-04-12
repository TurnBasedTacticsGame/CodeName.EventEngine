using System;

namespace CodeName.EventSystem.Tasks
{
    public class StateTaskCompletionSource
    {
        public bool IsCompleted { get; private set; }
        public Exception Exception { get; set; }

        public event Action Continuation;

        public void Complete()
        {
            IsCompleted = true;

            Continuation?.Invoke();
            Continuation = null;
        }
    }

    public class StateTaskCompletionSource<T> : StateTaskCompletionSource
    {
        public T Result { get; set; }

        public void Complete(T result)
        {
            Result = result;

            Complete();
        }
    }
}
