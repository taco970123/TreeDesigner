namespace TreeDesigner.Runtime
{
    [NodeName("SharedInt")]
    [NodePath("Base/Value/SharedValue/SharedIntNode")]
    public class SharedIntNode : ValueNode, IOutsideValueNode
    {
        public SharedIntValue sharedIntValue;
        [PortInfo("Int", 1, typeof(int)), ReadOnly]
        public int intValue;

        public object OutsideValue { get => sharedIntValue; set => sharedIntValue = (SharedIntValue)value; }

        protected override void OnGetValue()
        {
            intValue = sharedIntValue.Value;
        }
    }
}