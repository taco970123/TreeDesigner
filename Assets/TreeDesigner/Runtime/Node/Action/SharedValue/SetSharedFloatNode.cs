namespace TreeDesigner.Runtime
{
    [NodeName("SharedFloat")]
    [NodePath("Base/Action/SharedValue/SetSharedFloatNode")]
    public class SetSharedFloatNode : ActionNode, IOutsideValueNode
    {
        public SharedFloatValue sharedFloatValue;
        [PortInfo("Float", 0, typeof(float))]
        public float floatValue;

        public object OutsideValue { get => sharedFloatValue; set => sharedFloatValue = (SharedFloatValue)value; }

        protected override void DoAction()
        {  
            sharedFloatValue.Value = floatValue;
        }
    }
}