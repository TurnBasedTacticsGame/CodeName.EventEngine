namespace CodeName.EventSystem.State.Tasks
{
    public static class StateTaskUtility
    {
        public static void Forget(this StateTask task)
        {
            var awaiter = task.GetAwaiter();
            if (awaiter.IsCompleted)
            {
                awaiter.GetResult();
            }
            else
            {
                awaiter.OnCompleted(() =>
                {
                    awaiter.GetResult();
                });
            }
        }

        public static void Forget<T>(this StateTask<T> task)
        {
            var awaiter = task.GetAwaiter();
            if (awaiter.IsCompleted)
            {
                awaiter.GetResult();
            }
            else
            {
                awaiter.OnCompleted(() =>
                {
                    awaiter.GetResult();
                });
            }
        }
    }
}
