using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEngine.UIElements;
using UnityEditor;
using TreeDesigner.Editor;
#endif

namespace TreeDesigner.Runtime
{
    [InputPort(typeof(bool), "nodeType" ,ExposedPropertyNodeType.setvalue)]
    [NodeColor(74, 42, 192, "nodeType", ExposedPropertyNodeType.getvalue)]
    [NodeColor(239, 71, 111, "nodeType", ExposedPropertyNodeType.setvalue)]
    public abstract class ExposedPropertyNode : BaseNode
    {
        public enum ExposedPropertyNodeType
        {
            getvalue,
            setvalue,
        }

        [HideInInspector]
        public ExposedPropertyNodeType nodeType;
        
        protected virtual bool GetOnce => false;
        bool got;

        public sealed override BaseNode Clone()
        {
            ExposedPropertyNode cloneNode = Instantiate(this);
            cloneNode.name = name;
            return cloneNode;
        }
        public sealed override List<BaseNode> GetChildren()
        {
            return new List<BaseNode>();
        }

        protected sealed override void GetValue()
        {
            if(nodeType == ExposedPropertyNodeType.getvalue)
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
            else
            {
                base.GetValue();
            }
        }
        protected sealed override void OnReset() 
        {
            got = false;
        }
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
        public sealed override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);
            if (nodeType == ExposedPropertyNodeType.getvalue)
            {
                evt.menu.AppendAction("Convert to SetValue", (a) =>
                {
                    Undo.RecordObject(this, "Node (Change Type)");
                    nodeType = ExposedPropertyNodeType.setvalue;
                    ClearPorts();
                    TreeDesignerUtility.onNodeUpdate?.Invoke(this);
                    EditorUtility.SetDirty(this);
                });
            }
            else
            {
                evt.menu.AppendAction("Convert to GetValue", (a) =>
                {
                    Undo.RecordObject(this, "Node (Change Type)");
                    nodeType = ExposedPropertyNodeType.getvalue;
                    ClearPorts();
                    TreeDesignerUtility.onNodeUpdate?.Invoke(this);
                    EditorUtility.SetDirty(this);
                });
            }
        }

        public static Type GetCompatibleNode(Type exposedPropertyType)
        {
            var types = TypeCache.GetTypesDerivedFrom<ExposedPropertyNode>();
            foreach (var item in types)
            {
                var requireExposedpropertyTypes = item.GetCustomAttributes(typeof(RequireExposedpropertyType), false);
                if(requireExposedpropertyTypes.Length > 0 && (requireExposedpropertyTypes[0] as RequireExposedpropertyType).Type == exposedPropertyType)
                {
                    return item;
                }
            }
            return null;
        }
#endif
    }
}
