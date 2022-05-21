using System.Collections.Generic;

namespace TreeDesigner.Runtime
{
    [NodeColor(239, 71, 111), InputPort]
    public abstract class ActionNode : BaseNode, IActionNode
    {

        public sealed override BaseNode Clone()
        {
            ActionNode cloneNode = Instantiate(this);
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
            base.GetValue(); 
        }
        protected sealed override void OnReset() { }
        protected sealed override void OnStart()
        {
            GetValue();
            DoAction();
        }
        protected sealed override void OnStop() { }
        protected override State OnUpdate() 
        {
            return State.Success; 
        }

        protected abstract void DoAction();
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