using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
using TreeDesigner.Editor;
#endif

namespace TreeDesigner.Runtime
{
    [NodeName("SubTree")]
#if UNITY_EDITOR
    [NodeViewType(typeof(SubTreeNodeView))]
#endif
    [NodePath("Base/Utility/SubTreeNode")]
    [NodeColor(239, 71, 111, "HasOutputPort", false)]
    [NodeColor(255, 209, 102, "HasOutputPort", true)]
    public class SubTreeNode : BaseNode, IDecoratorNode, IOutsideValueNode
    {
        [SerializeField, HideInInspector]
        protected BaseNode child;

        public SubTree subTree;
        public object OutsideValue { get => subTree; set => subTree = (SubTree)value; }

        Dictionary<FieldInfo, ExposedProperty> outExpropertyFields;
        Dictionary<FieldInfo, ExposedProperty> inExpropertyFields;

        bool got;
        State lastState;

        public sealed override BaseNode Clone()
        {
            SubTreeNode cloneNode = Instantiate(this);
            cloneNode.name = name;
            cloneNode.child = null;
            cloneNode.subTree = subTree.Clone() as SubTree;
            cloneNode.subTree.holder = cloneNode;
            return cloneNode;
        }
        public sealed override List<BaseNode> GetChildren()
        {
            if (child != null)
                return new List<BaseNode> { child };
            else
                return new List<BaseNode>();
        }

        protected override FieldInfo OutFieldInfo(string name)
        {
            if(outExpropertyFields == null)
                outExpropertyFields = new Dictionary<FieldInfo, ExposedProperty>();
            ExposedProperty exposedProperty = subTree.OutExposedProperties.Find(i => i.Name == name);
            Type targetType = exposedProperty.GetType();
            FieldInfo fieldInfo = targetType.GetField("value", BindingFlags.NonPublic | BindingFlags.Instance);
            outExpropertyFields.Add(fieldInfo, exposedProperty);
            return fieldInfo;
        }
        protected override FieldInfo InFieldInfo(string name)
        {
            if (inExpropertyFields == null)
                inExpropertyFields = new Dictionary<FieldInfo, ExposedProperty>();
            ExposedProperty exposedProperty = subTree.InExposedProperties.Find(i => i.Name == name);
            Type targetType = exposedProperty.GetType();
            FieldInfo fieldInfo = targetType.GetField("value", BindingFlags.NonPublic | BindingFlags.Instance);
            inExpropertyFields.Add(fieldInfo, exposedProperty);
            return fieldInfo;
        }
        protected override object OutValue(FieldInfo fieldInfo)
        {
            return fieldInfo.GetValue(outExpropertyFields[fieldInfo]);
        }
        protected override void InValue(FieldInfo fieldInfo, object value)
        {
            fieldInfo.SetValue(inExpropertyFields[fieldInfo], value);
        }

        //protected sealed override void GetValue()
        //{
        //    if (!got)
        //    {
        //        got = true;
        //        base.GetValue();
        //    }
        //}
        protected sealed override void OnReset()
        {
            //got = false;
        }
        protected sealed override void OnStart()
        {
            GetValue();
            lastState = State.Success;
            subTree.ResetState();
        }
        protected sealed override void OnStop() { }
        protected override State OnUpdate()
        {
            if(subTree.treeState != State.Success && subTree.treeState != State.Failure)
                lastState = subTree.UpdateState();
            if (subTree.endNode && subTree.endNode.NodeState == State.Success)
                return child.Enable ? child.UpdateState() : State.Success;
            else
                return lastState;
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

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);
            evt.menu.AppendAction("Open SubTree", (a) =>
            {
                Selection.activeObject = subTree;
                AssetDatabase.OpenAsset(subTree.GetInstanceID());
            });
        }

        bool HasOutputPort()
        {
            return subTree?.endNode;
        }
#endif
    }
}
