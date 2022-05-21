namespace TreeDesigner.Runtime
{
    [NodeName("ExposedString")]
    [NodePath("Base/ExposedProperty/ExposedStringNode")]
    [RequireExposedpropertyType(typeof(ExposedStringProperty))]
    public class ExposedStringNode : ExposedPropertyNode,IOutsideValueNode
    {
        public ExposedStringProperty exposedProperty;
        public object OutsideValue { get => exposedProperty; set => exposedProperty = (ExposedStringProperty)value; }

        [PortInfo("String", 0, typeof(string)), PortIf("nodeType", ExposedPropertyNodeType.setvalue), ShowIf("nodeType", ExposedPropertyNodeType.setvalue)]
        public string inStringValue;
        [PortInfo("String", 1, typeof(string)), PortIf("nodeType", ExposedPropertyNodeType.getvalue), ShowIf("nodeType", ExposedPropertyNodeType.getvalue), ReadOnly]
        public string outStringValue;

        protected override void OnGetValue()
        {
            outStringValue = exposedProperty.Value;
        }
        protected override void DoAction()
        {
            exposedProperty.Value = inStringValue;
        }
    }
}
