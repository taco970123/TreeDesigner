namespace TreeDesigner.Runtime
{
    [NodeName("SharedBool")]
    [NodePath("Base/Action/SharedValue/SetSharedBoolNode")]
    public class SetSharedBoolNode : ActionNode, IOutsideValueNode
    {
        public SharedBoolValue sharedBoolValue;
        [PortInfo("Bool", 0, typeof(bool))]
        public bool boolValue;

        public object OutsideValue { get => sharedBoolValue; set => sharedBoolValue = (SharedBoolValue)value; }

        protected override void DoAction()
        {
            sharedBoolValue.Value = boolValue;
        }
    }
}