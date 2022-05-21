using System;

namespace TreeDesigner.Runtime
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class RequireExposedpropertyType : Attribute
    {
        Type type;
        public RequireExposedpropertyType(Type type)
        {
            this.type = type;
        }
        public Type Type => type;
    }
}
