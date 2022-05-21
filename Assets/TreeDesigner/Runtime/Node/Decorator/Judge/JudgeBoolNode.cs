namespace TreeDesigner.Runtime
{
    [NodeName("JudgeBool")]
    [NodePath("Base/Decorator/Judge/JudgeBoolNode")]
    public class JudgeBoolNode : JudgeNode
    {
        [PortInfo("Bool1", 0, typeof(bool))]
        public bool boolValue1;
        [PortInfo("Bool2", 0, typeof(bool))]
        public bool boolValue2;

        protected override void Judge()
        {
            isRight = boolValue1 == boolValue2;
        }
    }
}