namespace TreeDesigner.Runtime
{
    [NodeName("ExposedInt")]
    [NodePath("Base/ExposedProperty/ExposedIntNode")]
    [RequireExposedpropertyType(typeof(ExposedIntProperty))]
    public class ExposedIntNode : ExposedPropertyNode,IOutsideValueNode
    {
        public ExposedIntProperty exposedProperty;
        public object OutsideValue { get => exposedProperty; set => exposedProperty = (ExposedIntProperty)value; }

        [PortInfo("Int", 0, typeof(int)), PortIf("nodeType", ExposedPropertyNodeType.setvalue), ShowIf("nodeType", ExposedPropertyNodeType.setvalue)]
        public int inIntValue;
        [PortInfo("Int", 1, typeof(int)), PortIf("nodeType", ExposedPropertyNodeType.getvalue), ShowIf("nodeType", ExposedPropertyNodeType.getvalue), ReadOnly]
        public int outIntValue;

        protected override void OnGetValue()
        {
            outIntValue = (exposedProperty as ExposedIntProperty).Value;
        }
        protected override void DoAction()
        {
            (exposedProperty as ExposedIntProperty).Value = inIntValue;
        }
    }
}
