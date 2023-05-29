using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CodeName.EventSystem.State
{
    public abstract class EntityAsset : SerializedScriptableObject
    {
        [SerializeField] private Icon icon;
        [SerializeField] private string displayName;
        [TextArea(3, 10)]
        [SerializeField] private string description;

        public Icon Icon => icon;
        public string DisplayName => displayName;
        public string Description => description;

        public virtual void DefineInstanceData(InstanceDataCollection data, Func<EntityId> generateId) {}
        public virtual void GetDisplayContextData(EntityInstance instance, GameState state, Dictionary<string, object> entries) {}
    }
}
