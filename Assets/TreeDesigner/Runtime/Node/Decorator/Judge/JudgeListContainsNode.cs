using System.Collections;

namespace TreeDesigner.Runtime
{
    [NodeName("JudgeListContains")]
    [NodePath("Base/Decorator/Judge/JudgeListContainsNode")]
    public class JudgeListContainsNode : JudgeNode
    {
        [PortInfo("List", 0, typeof(IList)), ReadOnly]
        public IList valueList;
        [PortInfo("Value", 0, typeof(object)), ReadOnly]
        public object value;

        protected override void Judge()
        {
            isRight = valueList.Contains(value);
        }
    }
}