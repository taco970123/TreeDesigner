using System.Collections;
using UnityEngine;

namespace TreeDesigner.Runtime
{
    [NodeName("ListRandomElements")]
    [NodePath("Base/Operate/List/ListRandomElementsNode")]
    public class ListRandomElementsNode : OperateNode
    {
        public enum MNodeType { List, Single }
        [OnValueChanged("OnNodeTypeChanged")]
        public MNodeType nodeType;
        [PortInfo("List", 0, typeof(IList)), ReadOnly]
        public IList originalValueList;
        [PortInfo("List", 1, typeof(IList)), PortIf("nodeType", MNodeType.List), ReadOnly]
        public IList randomValueList;
        [PortInfo("Count", 0, typeof(int)), PortIf("nodeType", MNodeType.List), ShowIf("nodeType", MNodeType.List)]
        public int randomCount;
        [PortInfo("Value", 1, typeof(object)), PortIf("nodeType", MNodeType.Single), ReadOnly]
        public object value;

        protected override void Operate()
        {
            if (originalValueList.Count == 0)
                return;
            switch (nodeType)
            {
                case MNodeType.List:
                    randomValueList = System.Activator.CreateInstance(originalValueList.GetType()) as IList;
                    foreach (var item in originalValueList)
                    {
                        randomValueList.Add(item);
                    }
                    int tempCount = Mathf.Min(randomValueList.Count, randomCount);
                    while (tempCount < randomValueList.Count)
                    {
                        randomValueList.RemoveAt(Random.Range(0, randomValueList.Count));
                    }
                    break;
                case MNodeType.Single:
                    value = originalValueList[Random.Range(0, originalValueList.Count)];
                    break;
                default:
                    break;
            }
        }

#if UNITY_EDITOR
        void OnNodeTypeChanged()
        {
            ClearPorts();
            Editor.TreeDesignerUtility.onNodeUpdate?.Invoke(this);
        }
#endif
    }
}
