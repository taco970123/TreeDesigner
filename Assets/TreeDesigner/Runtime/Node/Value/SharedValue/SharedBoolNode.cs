namespace TreeDesigner.Runtime
{
    [NodeName("SharedBool")]
    [NodePath("Base/Value/SharedValue/SharedBoolNode")]
    public class SharedBoolNode : ValueNode ,IOutsideValueNode
    {
        public SharedBoolValue sharedBoolValue;
        [PortInfo("Bool", 1, typeof(bool)), ReadOnly]
        public bool boolValue;

        public object OutsideValue { get => sharedBoolValue; set => sharedBoolValue = (SharedBoolValue)value; }

        protected override void OnGetValue()
        {
            boolValue = sharedBoolValue.Value;
        }
    }
}