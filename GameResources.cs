using System.Collections.Generic;
using CodeName.EventSystem.Modding;
using Exanite.Core.Collections;
using Exanite.Core.Properties;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CodeName.EventSystem
{
    public class GameResources : SerializedScriptableObject
    {
        private static TwoWayDictionary<string, Object> Resources { get; } = new();

        public static IReadOnlyDictionary<string, Object> ResourcesByKey => Resources;

        [SerializeField] private List<ModInfo> mods = new();

        public void Initialize()
        {
            Resources.Clear();

            foreach (var mod in mods)
            {
                foreach (var resource in mod.Resources)
                {
                    Resources[resource.Key] = resource.Asset;
                }
            }
        }

        public static Object GetResource(string key)
        {
            return Resources[key];
        }

        public static T GetResource<T>(PropertyDefinition<T> resourceDefinition) where T : Object
        {
            return (T)GetResource(resourceDefinition.Key);
        }

        public static bool TryGetResource(string key, out Object resource)
        {
            return Resources.TryGetValue(key, out resource);
        }

        public static bool TryGetResourceKey(Object resource, out string key)
        {
            return Resources.Inverse.TryGetValue(resource, out key);
        }

        public static void Clear()
        {
            Resources.Clear();
        }
    }
}
