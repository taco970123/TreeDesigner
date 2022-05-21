using System.Collections;

namespace TreeDesigner.Runtime
{
    [NodeName("ListCount")]
    [NodePath("Base/Operate/List/ListCountNode")]
    public class ListCountNode : OperateNode
    {
        [PortInfo("List", 0, typeof(IList)), ReadOnly]
        public IList originalValueList;
        [PortInfo("Count", 1, typeof(int)), ReadOnly]
        public int count;

        protected override void Operate()
        {
            count = originalValueList.Count;
        }
    }
}