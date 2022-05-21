namespace TreeDesigner.Runtime
{
    [NodeName("SharedString")]
    [NodePath("Base/Action/SharedValue/SetSharedStringNode")]
    public class SetSharedStringNode : ActionNode, IOutsideValueNode
    {
        public SharedStringValue sharedStringValue;
        [PortInfo("String", 0, typeof(string))]
        public string stringValue;

        public object OutsideValue { get => sharedStringValue; set => sharedStringValue = (SharedStringValue)value; }

        protected override void DoAction()
        {
            sharedStringValue.Value = stringValue;
        }
    }
}