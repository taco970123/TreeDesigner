using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TreeDesigner.Runtime
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class NodeName : Attribute
    {
        string name;
        public NodeName(string name)
        {
            this.name = name;
        }
        public string Name => name;
    }
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class NodePath : Attribute
    {
        string path;

        public NodePath(string path)
        {
            this.path = path;
        }

        public string Path => path;
    }
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class NodeViewType : Attribute
    {
        Type type;
        string path;
        public NodeViewType(Type type, string path = "NodeView")
        {
            this.type = type;
            this.path = path;
        }
        public Type Type => type;
        public string Path => path;
    }
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class NodeColor : Attribute
    {
        Color color;
        string name;
        object condition;

        public NodeColor(float r, float g, float b)
        {
            color = new Color(r, g, b, 255);
        }
        public NodeColor(float r,float g,float b, string name, object condition)
        {
            color = new Color(r, g, b, 255);
            this.name = name;
            this.condition = condition;
        }

        public Color Color => color;
        public string Name => name;
        public object Condition => condition;
    }
}
