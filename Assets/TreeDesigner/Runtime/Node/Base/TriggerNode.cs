using System.Collections.Generic;
using UnityEngine;

namespace TreeDesigner.Runtime
{
    [NodeColor(217, 187, 249)]
    public abstract class TriggerNode : BaseNode
    {
        [SerializeField, HideInInspector]
        protected List<BaseNode> children = new List<BaseNode>();

        public sealed override BaseNode Clone()
        {
            TriggerNode cloneNode = Instantiate(this);
            cloneNode.name = name;
            cloneNode.children = new List<BaseNode>();
            return cloneNode;
        }
        public sealed override void AddChild(BaseNode baseNode)
        {
            if (CanAddChild && !children.Contains(baseNode))
                children.Add(baseNode);
        }
        public sealed override void RemoveChild(BaseNode baseNode)
        {
            if (children.Contains(baseNode))
                children.Remove(baseNode);
        }
        public sealed override List<BaseNode> GetChildren()
        {
            return children;
        }
        public override State UpdateState()
        {
            started = false;
            return base.UpdateState();
        }
        protected sealed override void GetValue()
        {
            base.GetValue();
        }
        protected sealed override void OnReset() { }
        protected sealed override void OnStart()
        {
            owner.OnStarted();
            GetValue();
        }
        protected sealed override void OnStop() 
        {
            owner.OnStopped();
        }
        protected sealed override State OnUpdate()
        {
            foreach (var item in children)
            {
                if (item.Enable)
                    item.UpdateState();
            }
            return State.Success;
        }

#if UNITY_EDITOR
        public sealed override void SwitchEnableState()
        {
            base.SwitchEnableState();
        }
        public sealed override void SetParentEnableState(bool parentEnable)
        {
            base.SetParentEnableState(parentEnable);
            children.ForEach((i) => i.SetParentEnableState(Enable && parentEnable));
        }
    #endif
    }
}