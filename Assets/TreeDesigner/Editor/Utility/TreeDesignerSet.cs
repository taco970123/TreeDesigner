#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TreeDesigner.Runtime;

namespace TreeDesigner.Editor
{
    [Serializable]
    public class NodeDescrip
    {
        public string className;
        public string displayName;
    }

    public class TreeDesignerSet : ScriptableObject
    {
        static TreeDesignerSet instance;
        public static TreeDesignerSet Instance 
        {
            get
            {
                if (instance == null)
                    instance = Resources.LoadAll<TreeDesignerSet>("Utility")[0];
                return instance;
            }
        }

        public bool showChinese;
        public List<NodeDescrip> nodeDescrips;
        
        void Init()
        {
            nodeDescrips = new List<NodeDescrip>();
            var types = TypeCache.GetTypesDerivedFrom<BaseNode>();
            foreach (var type in types)
            {
                if (type.IsAbstract)
                    continue;
                nodeDescrips.Add(new NodeDescrip { className = type.Name });
            }
        }

        public string NodeName(Type type)
        {
            string nodeName = string.Empty;
            NodeDescrip nodeDescrip = nodeDescrips.Find(i => i.className == type.Name);
            if (nodeDescrip != null)
                nodeName = nodeDescrip.displayName;
            return nodeName;
        }
    }
}

#endif