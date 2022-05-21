namespace TreeDesigner.Runtime
{
    [NodeName("SharedInt")]
    [NodePath("Base/Action/SharedValue/SetSharedIntNode")]
    public class SetSharedIntNode : ActionNode, IOutsideValueNode
    {
        public SharedIntValue sharedIntValue;
        [PortInfo("Int", 0, typeof(int))]
        public int intValue;

        public object OutsideValue { get => sharedIntValue; set => sharedIntValue = (SharedIntValue)value; }

        protected override void DoAction()
        {     
            sharedIntValue.Value = intValue;
        }
    }
}