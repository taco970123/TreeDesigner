using System.Collections;

namespace TreeDesigner.Runtime
{
    [NodeName("ListClear")]
    [NodePath("Base/Action/List/ListClearNode")]
    public class ListClearNode : ActionNode
    {
        [PortInfo("List", 0, typeof(IList), true)]
        public IList originalValueList;

        protected override void DoAction()
        {
            originalValueList?.Clear();
        }
    }
}