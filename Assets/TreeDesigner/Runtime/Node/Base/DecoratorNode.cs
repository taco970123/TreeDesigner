using System.Collections.Generic;
using UnityEngine;

namespace TreeDesigner.Runtime
{
    [NodeColor(255, 209, 102), InputPort, OutputPort]
    public abstract class DecoratorNode : BaseNode, IDecoratorNode
    {
        [SerializeField, HideInInspector]
        protected BaseNode child;
        public sealed override bool CanAddChild => true;
        public sealed override BaseNode Clone()
        {
            DecoratorNode cloneNode = Instantiate(this);
            cloneNode.name = name;
            cloneNode.child = null;
            return cloneNode;
        }
        public sealed override void AddChild(BaseNode baseNode)
        {
            if (CanAddChild)
                child = baseNode;
        }
        public sealed override void RemoveChild(BaseNode baseNode)
        {
            child = null;
        }
        public sealed override List<BaseNode> GetChildren()
        {
            if (child != null)
                return new List<BaseNode> { child };
            else
                return new List<BaseNode>();
        }

        protected sealed override void GetValue()
        {
            base.GetValue();
        }
        protected sealed override void OnReset() { }
        protected sealed override void OnStart()
        {
            GetValue();
            DoAction();
        }
        protected sealed override void OnStop() { }
        protected override State OnUpdate() { return child && child.Enable ? child.UpdateState() : State.Success; }
        protected abstract void DoAction();

#if UNITY_EDITOR
        public sealed override void SwitchEnableState()
        {
            base.SwitchEnableState();
        }
        public sealed override void SetParentEnableState(bool parentEnable)
        {
            base.SetParentEnableState(parentEnable);
            if (child != null)
                child.SetParentEnableState(Enable && parentEnable);
        }
    #endif
    }
}