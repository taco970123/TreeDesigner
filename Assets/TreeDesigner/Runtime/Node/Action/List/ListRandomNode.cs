using System.Collections;
using UnityEngine;

namespace TreeDesigner.Runtime
{
    [NodeName("ListRandom")]
    [NodePath("Base/Action/List/ListRandomNode")]
    public class ListRandomNode : ActionNode
    {
        [PortInfo("List", 0, typeof(IList), true)]
        public IList originalValueList;
        [PortInfo("Count", 0, typeof(int))]
        public int count;

        protected override void DoAction()
        {
            GetRandomList(originalValueList, count);
        }

        void GetRandomList(IList originalList, int count)
        {
            if (originalList.Count == 0)
                return;
            int tempCount = Mathf.Min(originalList.Count, count);
            while (tempCount < originalList.Count)
            {
                originalList.RemoveAt(Random.Range(0, originalList.Count));
            }
        }
    }
}