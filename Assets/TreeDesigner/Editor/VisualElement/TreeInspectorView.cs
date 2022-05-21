#if UNITY_EDITOR
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using TreeDesigner.Runtime;

namespace TreeDesigner.Editor
{
    public class TreeInspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<TreeInspectorView, UxmlTraits> { }

        protected VisualElement originalInspectorContainer;
        protected ExposedPropertyListView exposedPropertyListView;

        UnityEditor.Editor originalInspectorEditor;

        public TreeInspectorView()
        {
            VisualTreeAsset template = Resources.Load<VisualTreeAsset>("UIBuilder/TreeInspectorView");
            template.CloneTree(this);

            originalInspectorContainer = this.Q("original-inspector-container");
            exposedPropertyListView = this.Q<ExposedPropertyListView>("property-list-view");
        }

        public VisualElement TreeInspectorContainer => originalInspectorContainer;
        public ExposedPropertyListView ExposedPropertyListView { get => exposedPropertyListView; set => exposedPropertyListView = value; }


        public void Init(BaseTree tree,bool canPropertyInteract)
        {
            Object.DestroyImmediate(originalInspectorEditor);
            originalInspectorEditor = UnityEditor.Editor.CreateEditor(tree);
            IMGUIContainer info = new IMGUIContainer(() =>
            {
                if (originalInspectorEditor.target != null)
                    originalInspectorEditor.OnInspectorGUI();
            });
            info.name = "original-inspector-info";
            originalInspectorContainer.Clear();
            originalInspectorContainer.Add(info);
            exposedPropertyListView.Init(tree, canPropertyInteract);
        }
        public void ClearView()
        {
            originalInspectorContainer.Clear();
            exposedPropertyListView.ClearView();
        }
    }
}

#endif