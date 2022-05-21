using System.Collections.Generic;
using UnityEngine;

namespace TreeDesigner.Runtime
{
    [NodeColor(213, 57, 197)]
    public abstract class OperateNode : BaseNode
    {
        [ShowIf("ShowOperaterOnce", true)]
        public bool operateOnce;
        public virtual bool OperateOnce => operateOnce;
        public sealed override bool CanAddChild => false;
        
        bool operated;

        public sealed override BaseNode Clone()
        {
            OperateNode cloneNode = Instantiate(this);
            cloneNode.name = name;
            return cloneNode;
        }
        public sealed override void AddChild(BaseNode baseNode) { }
        public sealed override void RemoveChild(BaseNode baseNode) { }
        public sealed override List<BaseNode> GetChildren()
        {
            return new List<BaseNode>();
        }
        
        protected sealed override void GetValue()
        {
            nodeState = State.Success;
            base.GetValue();
            if (OperateOnce && !operated)
            {
                operated = true;
                Operate();
            }
            else if (!OperateOnce)
                Operate();
        }
        protected override void OnReset()
        {
            operated = false;
        }
        protected sealed override void OnStart() { }
        protected sealed override void OnStop() { }
        protected sealed override State OnUpdate()
        {
            return State.Default;
        }
        protected abstract void Operate();

#if UNITY_EDITOR
        public sealed override void SwitchEnableState()
        {
            base.SwitchEnableState();
        }
        public sealed override void SetParentEnableState(bool parentEnable)
        {
            base.SetParentEnableState(parentEnable);
        }

        public bool ShowOperaterOnce()
        {
            var propertyInfo = GetType().GetProperty("OperateOnce");
            return propertyInfo.GetGetMethod().DeclaringType == propertyInfo.GetGetMethod().GetBaseDefinition().DeclaringType;
        }
    #endif
    }
}