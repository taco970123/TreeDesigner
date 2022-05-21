namespace TreeDesigner.Runtime
{
    [NodeName("JudgeFloat")]
    [NodePath("Base/Decorator/Judge/JudgeFloatNode")]
    public class JudgeFloatNode : JudgeNode
    {
        [PortInfo("Float1", 0, typeof(float))]
        public float floatValue1;
        [PortInfo("Float2", 0, typeof(float))]
        public float floatValue2;
        [LabelAs("CompareType")]
        public BDCompareType compareType;

        protected override void Judge()
        {
            isRight = TreeDesignerRuntimeUtility.Compare(compareType, floatValue1, floatValue2);
        }
    }
}