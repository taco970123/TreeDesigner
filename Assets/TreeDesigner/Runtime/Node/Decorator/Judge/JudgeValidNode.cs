namespace TreeDesigner.Runtime
{
    [NodeName("JudgeValid")]
    [NodePath("Base/Decorator/Judge/JudgeValidNode")]
    public class JudgeValidNode : JudgeNode
    {
        [PortInfo("Value", 0, typeof(object))]
        public object value;

        protected override void Judge()
        {
            isRight = value != null;
        }
    }
}