#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using TreeDesigner.Runtime;

namespace TreeDesigner.Editor
{
    public class TreeFieldView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<TreeFieldView, UxmlTraits> { }

        BaseTree tree;
        ObjectField treeField;
        Button treeRemoveBtn;

        public TreeFieldView()
        {
            VisualTreeAsset template = Resources.Load<VisualTreeAsset>("UIBuilder/TreeFieldView");
            template.CloneTree(this);

            treeField = this.Q<ObjectField>("tree-field");
            treeField.RegisterCallback<MouseDownEvent>((e) => onSelectedEvent?.Invoke(tree));
            treeField.objectType = typeof(bool);
            treeRemoveBtn = this.Q<Button>("tree-remove-btn");
            treeRemoveBtn.clicked += () => onRemovedEvent?.Invoke(tree);
        }

        public BaseTree Tree => tree;

        public Action<BaseTree> onSelectedEvent;
        public Action<BaseTree> onRemovedEvent;
        
        public void Init(BaseTree tree)
        {
            this.tree = tree;
            treeField.SetValueWithoutNotify(tree);
        }
    }
}

#endif