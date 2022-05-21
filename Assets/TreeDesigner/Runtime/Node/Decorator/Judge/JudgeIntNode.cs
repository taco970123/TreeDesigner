namespace TreeDesigner.Runtime
{
    [NodeName("JudgeInt")]
    [NodePath("Base/Decorator/Judge/JudgeIntNode")]
    public class JudgeIntNode : JudgeNode
    {
        [PortInfo("Int1", 0, typeof(int))]
        public int intValue1;
        [PortInfo("Int2", 0, typeof(int))]
        public int intValue2;
        [LabelAs("CompareType")]
        public BDCompareType compareType;

        protected override void Judge()
        {
            isRight = TreeDesignerRuntimeUtility.Compare(compareType, intValue1, intValue2);
        }
    }
}
