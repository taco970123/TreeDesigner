namespace TreeDesigner.Runtime
{
    [NodeName("ExposedBool")]
    [NodePath("Base/ExposedProperty/ExposedBoolNode")]
    [RequireExposedpropertyType(typeof(ExposedBoolProperty))]
    public class ExposedBoolNode : ExposedPropertyNode,IOutsideValueNode
    {
        public ExposedBoolProperty exposedProperty;
        public object OutsideValue { get => exposedProperty; set => exposedProperty = (ExposedBoolProperty)value; }

        [PortInfo("Bool", 0, typeof(bool)), PortIf("nodeType", ExposedPropertyNodeType.setvalue) ,ShowIf("nodeType", ExposedPropertyNodeType.setvalue)]
        public bool inBoolValue;
        [PortInfo("Bool", 1, typeof(bool)), PortIf("nodeType", ExposedPropertyNodeType.getvalue), ShowIf("nodeType", ExposedPropertyNodeType.getvalue), ReadOnly]
        public bool outBoolValue;

        protected override void OnGetValue()
        {
            outBoolValue = exposedProperty.Value;
        }
        protected override void DoAction()
        {
            exposedProperty.Value = inBoolValue;
        }
    }
}
