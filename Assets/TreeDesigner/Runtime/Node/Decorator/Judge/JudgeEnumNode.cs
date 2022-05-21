using System;

namespace TreeDesigner.Runtime 
{
    [NodeName("JudgeEnum")]
    [NodePath("Base/Decorator/Judge/JudgeEnumNode")]
    public class JudgeEnumNode : JudgeNode
    {
        [PortInfo("Enum1", 0, typeof(Enum))]
        public Enum enum1;
        [PortInfo("Enum2", 0, typeof(Enum))]
        public Enum enum2;

#if UNITY_EDITOR
        [LabelAs("Enum1"), ReadOnly]
        public string name1;
        [LabelAs("Enum2"), ReadOnly]
        public string name2;
#endif

        protected override void OnGetValue()
        {
#if UNITY_EDITOR
            name1 = enum1.ToString();
            name2 = enum2.ToString();
#endif
        }
        protected override void Judge()
        {
            isRight = enum1.Equals(enum2);
        }
    }
}