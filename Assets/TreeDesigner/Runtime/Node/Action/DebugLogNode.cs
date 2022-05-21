using UnityEngine;

namespace TreeDesigner.Runtime
{
    [NodeName("DebugLog")]
    [NodePath("Base/Action/DebugLogNode")]
    public class DebugLogNode : ActionNode
    {
        [PortInfo("String", 0, typeof(string))]
        public string stringValue;

        protected override void DoAction()
        {
            Debug.Log(stringValue);
        }
    }
}
