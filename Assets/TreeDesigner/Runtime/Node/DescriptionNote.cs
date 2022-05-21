using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TreeDesigner.Runtime
{
    public class DescriptionNote : ScriptableObject
    {
        public string title;
        public string content;

#if UNITY_EDITOR
        [SerializeField, HideInInspector]
        Rect position;
        public Rect Position { get => position; set => position = value; }
#endif
    }
}