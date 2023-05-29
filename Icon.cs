using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CodeName.EventSystem
{
    [InlineProperty(LabelWidth = 50)]
    [Serializable]
    public class Icon
    {
        [SerializeField] private Sprite sprite;
        [SerializeField] private GameObject prefab;

        public Sprite Sprite => sprite;
        public GameObject Prefab => prefab;
    }
}
