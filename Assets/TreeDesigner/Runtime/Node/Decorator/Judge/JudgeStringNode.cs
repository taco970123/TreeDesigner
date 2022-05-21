namespace TreeDesigner.Runtime
{
    [NodeName("JudgeString")]
    [NodePath("Base/Decorator/Judge/JudgeStringNode")]
    public class JudgeStringNode : JudgeNode
    {
        [PortInfo("String1", 0, typeof(string))]
        public string stringValue1;
        [PortInfo("String2", 0, typeof(string))]
        public string stringValue2;

        protected override void Judge()
        {
            isRight = stringValue1 == stringValue2;
        }
    }
}