using System.Collections.Generic;
using UnityEngine;

namespace TreeDesigner.Runtime
{
    [NodeColor(6, 214, 160), InputPort, OutputPort(1)]
    public abstract class CompositeNode : BaseNode, ICompositeNode
    {
        [SerializeField, HideInInspector]
        protected List<BaseNode> children = new List<BaseNode>();

        public sealed override BaseNode Clone()
        {
            CompositeNode cloneNode = Instantiate(this);
            cloneNode.name = name;
            cloneNode.children = new List<BaseNode>();
            //cloneNode.originalNode = this;
            //for (int i = 0; i < children.Count; i++)
            //{
            //    compositeNode.children[i] = children[i].Clone();
            //}
            return cloneNode;
        }
        public sealed override List<BaseNode> GetChildren()
        {
            return children;
        }

        protected sealed override void GetValue()
        {
            base.GetValue();
        }
        protected sealed override void OnStart()
        {
            GetValue();
        }
        protected override void OnStop() { }
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