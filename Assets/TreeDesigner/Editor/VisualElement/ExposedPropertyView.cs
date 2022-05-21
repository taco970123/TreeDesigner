#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using TreeDesigner.Runtime;

namespace TreeDesigner.Editor
{
    public class ExposedPropertyView : VisualElement, IDragableVisualElement
    {
        public new class UxmlFactory : UxmlFactory<ExposedPropertyView, UxmlTraits> { }

        ExposedProperty exposedProperty;
        Label nameLabel;
        Label typeLabel;
        VisualElement originalEditorContainer;
        DragHandle dragHandle;
        UnityEditor.Editor originalPropertyEditor;

        public ExposedPropertyView()
        {
            VisualTreeAsset template = Resources.Load<VisualTreeAsset>("UIBuilder/ExposedPropertyView");
            template.CloneTree(this);
            
            typeLabel = this.Q<Label>("property-type-label");
            typeLabel.RegisterCallback<MouseDownEvent>((e) => 
            {
                if(e.button == 0)
                    AddToClassList("dragged");
            });
            typeLabel.RegisterCallback<MouseUpEvent>((e) => 
            {
                if (e.button == 0)
                    RemoveFromClassList("dragged");
            });

            nameLabel = this.Q<Label>("property-name-label");
            nameLabel.bindingPath = "Name";
            nameLabel.RegisterCallback<ChangeEvent<string>>(NameChangeCallback);

            var showInfoBtn = this.Q<Button>("property-showInfo-btn");
            showInfoBtn.clicked += () =>
            {
                exposedProperty.ShowInfo = !exposedProperty.ShowInfo;
            };

            var removeBtn = this.Q<Button>("property-remove-btn");
            removeBtn.clicked += () => onRemovedEvent?.Invoke(exposedProperty);

            originalEditorContainer = this.Q("original-editor-container");

            dragHandle?.Dispose();
            dragHandle = new DragHandle();
            dragHandle.Init(typeLabel, this);

            this.AddManipulator(new ContextualMenuManipulator((ContextualMenuPopulateEvent evt) => onDisplayMenuEvent?.Invoke(evt)));
        }

        public VisualElement OriginalEditorContainer => originalEditorContainer;
        public ExposedProperty ExposedProperty => exposedProperty;
        public Action<ExposedProperty> onRemovedEvent;
        public Action<ContextualMenuPopulateEvent> onDisplayMenuEvent;

        public void Init(ExposedProperty exposedProperty,bool canInteract)
        {
            if (this.exposedProperty)
                this.exposedProperty.onValueChangedEvent -= UpdateView;

            this.exposedProperty = exposedProperty;
            this.exposedProperty.onValueChangedEvent += UpdateView;

            nameLabel.Unbind();
            nameLabel.Bind(new SerializedObject(exposedProperty));
            nameLabel.text = exposedProperty.Name;

            var removeBtn = this.Q<Button>("property-remove-btn");
            removeBtn.style.display = canInteract ? DisplayStyle.Flex : DisplayStyle.None;

            UnityEngine.Object.DestroyImmediate(originalPropertyEditor);
            originalPropertyEditor = UnityEditor.Editor.CreateEditor(exposedProperty);
            originalEditorContainer.Clear();
            IMGUIContainer info = new IMGUIContainer(() =>
            {
                if (originalPropertyEditor.target != null)
                    originalPropertyEditor.OnInspectorGUI();
            });
            info.name = "original-editor-info";
            originalEditorContainer.Add(info);
            UpdateView();
        }
        
        Vector2 localMousePosition;
        public void StartDrag(object dragArea)
        {
            AddToClassList("dragged");
            if (dragArea is TreeView treeView)
            {
                if (treeView.Tree.exposedProperties.Contains(exposedProperty))
                    DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
            }
        }
        public void StopDrag(object dragArea)
        {
            RemoveFromClassList("dragged");
        }
        public void PerformDrag(DragPerformEvent e, object dragArea)
        {
            if(dragArea is TreeView treeView)
            {
                if (treeView.Tree.exposedProperties.Contains(exposedProperty))
                    treeView.panel.contextualMenuManager.DisplayMenu(e, treeView);
            }
            localMousePosition = e.localMousePosition;
        }
        public void BuildContextMenu(ContextualMenuPopulateEvent e, object dragArea)
        {
            if (dragArea is TreeView treeView)
            {
                e.menu.AppendAction("Get Shared Value", (a) =>
                {
                    ExposedPropertyNode sharedValueNode = treeView.CreateNodeWithOutsideValue(ExposedPropertyNode.GetCompatibleNode(exposedProperty.GetType()), localMousePosition, exposedProperty) as ExposedPropertyNode;
                    sharedValueNode.nodeType = ExposedPropertyNode.ExposedPropertyNodeType.getvalue;
                    sharedValueNode.ClearPorts();
                    TreeDesignerUtility.onNodeUpdate?.Invoke(sharedValueNode);
                });
                e.menu.AppendAction("Set Shared Value", (a) =>
                {
                    ExposedPropertyNode sharedValueNode = treeView.CreateNodeWithOutsideValue(ExposedPropertyNode.GetCompatibleNode(exposedProperty.GetType()), localMousePosition, exposedProperty) as ExposedPropertyNode;
                    sharedValueNode.nodeType = ExposedPropertyNode.ExposedPropertyNodeType.setvalue;
                    sharedValueNode.ClearPorts();
                    TreeDesignerUtility.onNodeUpdate?.Invoke(sharedValueNode);
                });
            } 
        }
        
        void UpdateView()
        {
            RemoveFromClassList("showInfo");
            if (exposedProperty.ShowInfo)
                AddToClassList("showInfo");
            SetName();
            SetColor();
            originalEditorContainer.SetEnabled(exposedProperty.Interactable);
        }

        void SetName()
        {
            if (exposedProperty.GetType().GetCustomAttributes(typeof(ExposedPropertyName), false).Length > 0)
                typeLabel.text = (exposedProperty.GetType().GetCustomAttributes(typeof(ExposedPropertyName), false)[0] as ExposedPropertyName).Name;
        }
        void SetColor()
        {
            if (exposedProperty.GetType().GetCustomAttributes(typeof(ExposedPropertyColor), false).Length > 0)
                typeLabel.style.backgroundColor = (exposedProperty.GetType().GetCustomAttributes(typeof(ExposedPropertyColor), false)[0] as ExposedPropertyColor).Color/255;
        }
        void NameChangeCallback(ChangeEvent<string> evt)
        {
            if (evt == null)
                return;
            string newValue = evt.newValue;
            if (newValue == "")
                newValue = "New Exposed Property";
            ExposedProperty sameName = ExposedProperty.exposedProperties.Find(i => i.Name == newValue && i != exposedProperty);
            if (sameName)
            {
                newValue += "(1)";
                sameName = ExposedProperty.exposedProperties.Find(i => i.Name == newValue);
            }
            exposedProperty.Name = newValue;
        }
    }
}

#endif