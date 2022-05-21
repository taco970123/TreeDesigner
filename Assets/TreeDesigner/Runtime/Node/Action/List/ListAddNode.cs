using System.Collections;

namespace TreeDesigner.Runtime
{
    [NodeName("ListAdd")]
    [NodePath("Base/Action/List/ListAddNode")]
    public class ListAddNode : ActionNode
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
                    if (!originalValueList.Contains(item))
                        originalValueList.Add(item);
                }
            }
            else if(!originalValueList.Contains(value))
                originalValueList.Add(value);
        }
    }
}