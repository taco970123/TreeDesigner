using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TreeDesigner.Runtime
{
    [NodeName("SubTreeEntry"), NodeColor(217, 187, 249), OutputPort]
    [NodePath("Sub/Utility/SubTreeEntryNode")]
    public class SubTreeEntryNode : BaseNode
    {
        [SerializeField, HideInInspector]
        protected BaseNode child;

        public sealed override BaseNode Clone()
        {
            SubTreeEntryNode cloneNode = Instantiate(this);
            cloneNode.name = name;
            cloneNode.child = null;
            return cloneNode;
        }
        public sealed override List<BaseNode> GetChildren()
        {
            if (child != null)
                return new List<BaseNode> { child };
            else
                return new List<BaseNode>();
        }

        protected sealed override void GetValue() { }
        protected sealed override void OnReset() { }
        protected sealed override void OnStart() { }
        protected sealed override void OnStop() { }
        protected sealed override State OnUpdate()
        {
            return child && child.Enable ? child.UpdateState() : State.Success;
        }
        public sealed override void AddChild(BaseNode baseNode)
        {
            child = baseNode;
        }
        public sealed override void RemoveChild(BaseNode baseNode)
        {
            child = null;
        }

#if UNITY_EDITOR
        public sealed override bool CanAddChild => true;
        public sealed override bool IsCopiable => false;

        public sealed override void SwitchEnableState() { }
        public sealed override void SetParentEnableState(bool parentEnable) { }
#endif
    }
}