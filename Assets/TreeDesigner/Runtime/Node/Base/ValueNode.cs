using System.Collections.Generic;

namespace TreeDesigner.Runtime
{
    [NodeColor(74, 42, 192)]
    public abstract class ValueNode : BaseNode
    {
        protected virtual bool GetOnce => false;

        bool got;

        public sealed override BaseNode Clone()
        {
            ValueNode cloneNode = Instantiate(this);
            cloneNode.name = name;
            
            //cloneNode.originalNode = this;
            return cloneNode;
        }
        public sealed override List<BaseNode> GetChildren()
        {
            return new List<BaseNode>();
        }

        protected sealed override void GetValue()
        {
            nodeState = State.Success;
            if (GetOnce && !got)
            {
                got = true;
                OnGetValue();
            }
            else if (!GetOnce)
            {
                OnGetValue();
            }
        }
        protected sealed override void OnReset()
        {
            got = false;
        }
        protected sealed override void OnStart(){ }
        protected sealed override void OnStop() { }
        protected sealed override State OnUpdate()
        {
            return State.Success;
        }
        public sealed override void AddChild(BaseNode baseNode) { }
        public sealed override void RemoveChild(BaseNode baseNode) { }

#if UNITY_EDITOR
        public sealed override bool CanAddChild => false;

        public sealed override void SwitchEnableState()
        {
            base.SwitchEnableState();
        }
        public sealed override void SetParentEnableState(bool parentEnable)
        {
            base.SetParentEnableState(parentEnable);
        }
    #endif
    }
}