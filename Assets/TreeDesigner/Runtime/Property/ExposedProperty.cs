using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
using TreeDesigner.Editor;
#endif

namespace TreeDesigner.Runtime
{
    [HideScriptField]
    public abstract class ExposedProperty : ScriptableObject
#if UNITY_EDITOR
    , IDragableObejct
#endif
    {
        public string Name;

        [HideInInspector]
        public bool Global;
        public abstract Type Type { get; }

        public ExposedProperty Clone()
        {
            return Instantiate(this);
        }

        public abstract object GetValue();
        public abstract void SetValue(object value);

        static Dictionary<ExposedProperty, ExposedProperty> m_RuntimeGlobalPropertyPair = new Dictionary<ExposedProperty, ExposedProperty>();
        public static Dictionary<ExposedProperty, ExposedProperty> RuntimeGlobalPropertyPair => m_RuntimeGlobalPropertyPair;

        public static ExposedProperty GetRuntimeGlobalProperty(ExposedProperty exposedProperty)
        {
            if (!m_RuntimeGlobalPropertyPair.ContainsKey(exposedProperty))
                m_RuntimeGlobalPropertyPair.Add(exposedProperty, exposedProperty.Clone());
            return m_RuntimeGlobalPropertyPair[exposedProperty];
        }



#if UNITY_EDITOR
        [SerializeField, HideInInspector]
        bool showInfo;
        public bool ShowInfo
        {
            get => showInfo;
            set
            {
                showInfo = value;
                onValueChangedEvent?.Invoke();
            }
        }
        [SerializeField, HideInInspector]
        bool interactable = true;
        public bool Interactable
        {
            get => interactable;
            set
            {
                interactable = value;
                onValueChangedEvent?.Invoke();
            }
        }
        public static List<ExposedProperty> exposedProperties = new List<ExposedProperty>();
        public static void UpdateAllProperties(List<ExposedProperty> exposedProperties)
        {
            ExposedProperty.exposedProperties = exposedProperties;
        }

        Vector2 localMousePosition;
        public void StartDrag(object dragArea)
        {
            if (Global && dragArea is TreeView treeView && treeView.Tree)
                DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
        }

        public void StopDrag(object dragArea)
        {
        }

        public void PerformDrag(DragPerformEvent e, object dragArea)
        {
            if (Global && dragArea is TreeView treeView && treeView.Tree)
                treeView.panel.contextualMenuManager.DisplayMenu(e, treeView);
            localMousePosition = e.localMousePosition;
        }

        public void BuildContextMenu(ContextualMenuPopulateEvent e, object dragArea)
        {
            if (Global && dragArea is TreeView treeView && treeView.Tree)
            {
                e.menu.AppendAction("Get Shared Value", (a) =>
                {
                    ExposedPropertyNode sharedValueNode = treeView.CreateNodeWithOutsideValue(ExposedPropertyNode.GetCompatibleNode(GetType()), localMousePosition, this) as ExposedPropertyNode;
                    sharedValueNode.nodeType = ExposedPropertyNode.ExposedPropertyNodeType.getvalue;
                    sharedValueNode.ClearPorts();
                    TreeDesignerUtility.onNodeUpdate?.Invoke(sharedValueNode);
                });
                e.menu.AppendAction("Set Shared Value", (a) =>
                {
                    ExposedPropertyNode sharedValueNode = treeView.CreateNodeWithOutsideValue(ExposedPropertyNode.GetCompatibleNode(GetType()), localMousePosition, this) as ExposedPropertyNode;
                    sharedValueNode.nodeType = ExposedPropertyNode.ExposedPropertyNodeType.setvalue;
                    sharedValueNode.ClearPorts();
                    TreeDesignerUtility.onNodeUpdate?.Invoke(sharedValueNode);
                });
            }
        }

        public Action onValueChangedEvent;
#endif
    }
}