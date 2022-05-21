using System;
using System.Collections;
using System.Collections.Generic;

namespace TreeDesigner.Runtime
{
    [NodeName("ToList")]
    [NodePath("Base/Operate/Convert/ToListNode")]
    public class ToListNode : OperateNode
    {
        public override bool OperateOnce => OnlyOperateOnce;

        [PortInfo("Value", 0, typeof(object)), ReadOnly]
        public object value;
        [PortInfo("List", 1, typeof(IList)), ReadOnly]
        public IList valueList;

        public bool OnlyOperateOnce = true;

        Type m_TargetType;

        protected override void OnReset()
        {
            base.OnReset();
            if (linkValues == null) return;
            var item = linkValues.Find(i => i.Item2.Name == "value");
            if(item == default) return;
            if(item.Item1 == null) return;
            m_TargetType = item.Item1.FieldType;
        }
        protected override void Operate()
        {
            Type listType = typeof(List<>).MakeGenericType(new[] { m_TargetType });
            valueList = Activator.CreateInstance(listType) as IList;
            valueList.Add(value);
        }
    }
}