using System;

namespace TreeDesigner.Runtime
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class PortInfo : Attribute
    {
        string portName;
        int direction;
        Type portType;
        bool required;

        string requiredReference;
        object requiredCondition;

        public string PortName => portName;
        public int Direction => direction;
        public Type PortType => portType;
        public bool Required => required;
        public string RequiredReference => requiredReference;
        public object RequiredCondition => requiredCondition;


        public PortInfo(string portName, int direction, Type portType, bool required = false)
        {
            this.portName = portName;
            this.direction = direction;
            this.portType = portType;
            this.required = required;
        }

        public PortInfo(string portName, int direction, Type portType, string requiredReference, object requiredCondition)
        {
            this.portName = portName;
            this.direction = direction;
            this.portType = portType;
            this.requiredReference = requiredReference;
            this.requiredCondition = requiredCondition;
        }

    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class PortIf : Attribute
    {
        protected string name;
        protected object condition;

        public PortIf(string name, object condition)
        {
            this.name = name;
            this.condition = condition;
        }

        public string Name => name;
        public object Condition => condition;
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class InputPort : Attribute
    {
        Type type;
        string name;
        object condition;

        public InputPort()
        {
            type = typeof(bool);
        }
        public InputPort(Type type)
        {
            this.type = type;
        }
        public InputPort(Type type,string name,object condition)
        {
            this.type = type;
            this.name = name;
            this.condition = condition;
        }

        public Type Type => type;
        public string Name => name;
        public object Condition => condition;
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class OutputPort : Attribute
    {
        Type type;
        int capacity;
        string name;
        object condition;

        public OutputPort()
        {
            capacity = 0;
            type = typeof(bool);
        }
        public OutputPort(int capacity)
        {
            this.capacity = capacity;
            type = typeof(bool);
        }
        public OutputPort(Type type)
        {
            capacity = 0;
            this.type = type;
        }
        public OutputPort(Type type, int capacity)
        {
            this.type = type;
            this.capacity = capacity;
        }
        public OutputPort(Type type, string name, object condition)
        {
            this.type = type;
            capacity = 0;
            this.name = name;
            this.condition = condition;
        }
        public OutputPort(Type type, int capacity, string name, object condition)
        {
            this.type = type;
            this.capacity = capacity;
            this.name = name;
            this.condition = condition;
        }
        
        public Type Type => type;
        public int Capacity => capacity;
        public string Name => name;
        public object Condition => condition;
    }
}
