using System.Collections;

namespace TreeDesigner.Runtime
{
    [NodeName("ExposedList")]
    [NodePath("Base/ExposedProperty/ExposedListNode")]
    [RequireExposedpropertyType(typeof(ExposedListProperty))]
    public class ExposedListNode : ExposedPropertyNode,IOutsideValueNode
    {
        public ExposedListProperty exposedProperty;
        public object OutsideValue { get => exposedProperty; set => exposedProperty = (ExposedListProperty)value; }

        [PortInfo("List", 0, typeof(IList)), ReadOnly, PortIf("nodeType", ExposedPropertyNodeType.setvalue)]
        public IList inListValue;
        [PortInfo("List", 1, typeof(IList)), ReadOnly, PortIf("nodeType", ExposedPropertyNodeType.getvalue)]
        public IList outListValue;

        protected override void OnGetValue()
        {
            outListValue = (exposedProperty as ExposedListProperty).Value;
        }
        protected override void DoAction()
        {
            (exposedProperty as ExposedListProperty).Value = inListValue;
        }
    }
}
