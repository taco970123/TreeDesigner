namespace TreeDesigner.Runtime
{
    [NodeName("SharedString")]
    [NodePath("Base/Value/SharedValue/SharedStringNode")]
    public class SharedStringNode : ValueNode ,IOutsideValueNode
    {
        public SharedStringValue sharedStringValue;
        [PortInfo("String", 1, typeof(string)), ReadOnly]
        public string stringValue;

        public object OutsideValue { get => sharedStringValue; set => sharedStringValue = (SharedStringValue)value; }

        protected override void OnGetValue()
        {
            stringValue = sharedStringValue.Value;
        }
    }
}