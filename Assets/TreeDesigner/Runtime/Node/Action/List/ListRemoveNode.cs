using System.Collections;

namespace TreeDesigner.Runtime
{
    [NodeName("ListRemove")]
    [NodePath("Base/Action/List/ListRemoveNode")]
    public class ListRemoveNode : ActionNode
    {
        [PortInfo("List", 0, typeof(IList), true)]
        public IList originalValueList;
        [PortInfo("Value", 0, typeof(object), true)]
        public object value;

        protected override void DoAction()
        {
            if (value == null) return;
            if (value is IList list)
            {
                foreach (var item in list)
                {
                    if (originalValueList.Contains(item))
                        originalValueList.Remove(item);
                }
            }
            else if (originalValueList.Contains(value))
                originalValueList.Remove(value);
        }
    }
}