#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using TreeDesigner.Runtime;

namespace TreeDesigner.Editor
{
    public class NodeInfoView : Node
    {
        UnityEditor.Editor editor;

        public NodeInfoView(string path) : base(path)
        {
        }
    
        public bool isHide => style.display == DisplayStyle.None;

        public override bool IsSelectable()
        {
            return false;
        }
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.StopImmediatePropagation();
        }
        public void Init(NodeView nodeView)
        {
            Clear();
            nodeView.onGeometryChanged = UpdatePosition;

            UnityEngine.Object.DestroyImmediate(editor);
            editor = UnityEditor.Editor.CreateEditor(nodeView.node);
            IMGUIContainer container = new IMGUIContainer(() =>
            {
                if (editor.target != null)
                    editor.OnInspectorGUI();
            });
            container.AddToClassList("nodeInfo");
            Add(container);
        
            Rect newRect = nodeView.GetPosition();
            newRect.xMin += nodeView.GetPosition().width;
            SetPosition(newRect);
            BringToFront();
            style.display = DisplayStyle.Flex;

            SetupClassed(nodeView.node);
        }
        public void Hide()
        {
            Clear();
            style.display = DisplayStyle.None;
        }
        public void UpdatePosition(NodeView nodeView)
        {
            Rect newRect = nodeView.GetPosition();
            newRect.xMin += nodeView.GetPosition().width;
            SetPosition(newRect);
        }
        void SetupClassed(BaseNode node)
        {
            RemoveFromClassList("root");
            RemoveFromClassList("action");
            RemoveFromClassList("decorator");
            RemoveFromClassList("composite");
            RemoveFromClassList("value");
            RemoveFromClassList("operate");
            RemoveFromClassList("trigger");
            switch (node)
            {
                case IRootNode rootNode:
                    AddToClassList("root");
                    break;
                case IActionNode actionNode:
                    AddToClassList("action");
                    break;
                case IDecoratorNode decoratorNode:
                    AddToClassList("decorator");
                    break;
                case ICompositeNode compositeNode:
                    AddToClassList("composite");
                    break;
                case ValueNode valueNode:
                    AddToClassList("value");
                    break;
                case OperateNode operateNode:
                    AddToClassList("operate");
                    break;
                case TriggerNode triggerNode:
                    AddToClassList("trigger");
                    break;
            }
        }
    }
}
#endif