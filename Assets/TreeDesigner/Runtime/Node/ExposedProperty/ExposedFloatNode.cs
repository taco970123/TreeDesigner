namespace TreeDesigner.Runtime
{
    [NodeName("ExposedFloat")]
    [NodePath("Base/ExposedProperty/ExposedFloatNode")]
    [RequireExposedpropertyType(typeof(ExposedFloatProperty))]
    public class ExposedFloatNode : ExposedPropertyNode, IOutsideValueNode
    {
        public ExposedFloatProperty exposedProperty;
        public object OutsideValue { get => exposedProperty; set => exposedProperty = (ExposedFloatProperty)value; }

        [PortInfo("Float", 0, typeof(float)), PortIf("nodeType", ExposedPropertyNodeType.setvalue), ShowIf("nodeType", ExposedPropertyNodeType.setvalue)]
        public float inFloatValue;
        [PortInfo("Float", 1, typeof(float)), PortIf("nodeType", ExposedPropertyNodeType.getvalue), ShowIf("nodeType", ExposedPropertyNodeType.getvalue), ReadOnly]
        public float outFloatValue;

        protected override void OnGetValue()
        {
            outFloatValue = exposedProperty.Value;
        }
        protected override void DoAction()
        {
            exposedProperty.Value = inFloatValue;
        }
    }
}
