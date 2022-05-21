using System.Collections;

namespace TreeDesigner.Runtime
{
    [NodeName("ListClone")]
    [NodePath("Base/Operate/List/ListCloneNode")]
    public class ListCloneNode : OperateNode
    {
        public override bool OperateOnce => true;

        [PortInfo("List", 0, typeof(IList), true)]
        public IList originalValueList;
        [PortInfo("List", 1, typeof(IList), true)]
        public IList newValueList;

        protected override void Operate()
        {
            newValueList = System.Activator.CreateInstance(originalValueList.GetType()) as IList;
            foreach (var item in originalValueList)
            {
                newValueList.Add(item);
            }
        }
    }
}