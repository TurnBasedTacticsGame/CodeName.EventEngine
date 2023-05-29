using System;
using System.Collections.Generic;

namespace CodeName.EventSystem
{
    public class InstanceDataCollection
    {
        private readonly Dictionary<Type, InstanceData> entries = new();

        public IReadOnlyDictionary<Type, InstanceData> Entries => entries;

        public void Define<T>() where T : InstanceData, new()
        {
            entries.Add(typeof(T), new T());
        }

        public void Define<T>(T data) where T : InstanceData
        {
            entries.Add(typeof(T), data);
        }

        public void Define<T>(Func<T> createData) where T : InstanceData
        {
            entries.Add(typeof(T), createData());
        }

        public T Get<T>() where T : InstanceData
        {
            return (T)entries[typeof(T)];
        }

        public bool TryGet<T>(out T data) where T : InstanceData
        {
            if (entries.TryGetValue(typeof(T), out var untypedData))
            {
                data = (T)untypedData;
            }

            data = null;

            return false;
        }
    }
}
