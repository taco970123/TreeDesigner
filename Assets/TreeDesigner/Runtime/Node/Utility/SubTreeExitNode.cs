using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TreeDesigner.Runtime
{
    [NodeName("SubTreeExit"), NodeColor(217, 187, 249), InputPort]
    [NodePath("Sub/Utility/SubTreeExitNode")]
    public class SubTreeExitNode : BaseNode, IRootNode
    {
        public sealed override BaseNode Clone()
        {
            SubTreeExitNode rootNode = Instantiate(this);
            rootNode.name = name;
            return rootNode;
        }
        public sealed override List<BaseNode> GetChildren()
        {
            return new List<BaseNode>();
        }

        protected sealed override void GetValue() { }
        protected sealed override void OnReset() { }
        protected sealed override void OnStart() { }
        protected sealed override void OnStop() { }
        protected sealed override State OnUpdate()
        {
            return State.Success;
        }
        public sealed override void AddChild(BaseNode baseNode) { }
        public sealed override void RemoveChild(BaseNode baseNode) { }

#if UNITY_EDITOR
        public sealed override bool CanAddChild => false;
        public sealed override bool IsCopiable => false;

        public sealed override void SwitchEnableState() { }
        public sealed override void SetParentEnableState(bool parentEnable) { }
#endif
    }
}
