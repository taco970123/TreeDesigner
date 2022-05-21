using System;
using UnityEngine;

namespace TreeDesigner.Runtime
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class ExposedPropertyColor : Attribute
    {
        Color color;
        public ExposedPropertyColor(float r, float g, float b)
        {
            color = new Color(r, g, b, 255);
        }
        public Color Color => color;
    }
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class ExposedPropertyName : Attribute
    {
        string name;
        public ExposedPropertyName(string name)
        {
            this.name = name;
        }
        public string Name => name;
    }
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class ExposedPropertyOrder : Attribute
    {
        int index;
        public ExposedPropertyOrder(int index)
        {
            this.index = index;
        }
        public int Index => index;
    }
}
