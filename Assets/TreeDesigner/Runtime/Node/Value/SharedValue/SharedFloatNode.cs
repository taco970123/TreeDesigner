namespace TreeDesigner.Runtime
{
    [NodeName("SharedFloat")]
    [NodePath("Base/Value/SharedValue/SharedFloatNode")]
    public class SharedFloatNode : ValueNode, IOutsideValueNode
    {
        public SharedFloatValue sharedFloatValue;
        [PortInfo("Float", 1, typeof(float)), ReadOnly]
        public float floatValue;

        public object OutsideValue { get => sharedFloatValue; set => sharedFloatValue = (SharedFloatValue)value; }

        protected override void OnGetValue()
        {
            floatValue = sharedFloatValue.Value;
        }
    }
}