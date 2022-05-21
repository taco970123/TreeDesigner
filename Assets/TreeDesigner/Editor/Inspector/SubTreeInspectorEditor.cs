#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.UIElements;
using TreeDesigner.Runtime;
using UnityEngine;
using System;

namespace TreeDesigner.Editor
{
    [CustomEditor(typeof(SubTree))]
    public class SubTreeInspectorEditor : TreeInspectorEditor
    {
        SubTree tree;

        protected virtual void OnEnable()
        {
            tree = target as SubTree;
        }

        public override VisualElement CreateInspectorGUI()
        {
            treeInspectorView?.ExposedPropertyListView.Dispose();
            treeInspectorView = new TreeInspectorView();
            var parent = treeInspectorView.ExposedPropertyListView.parent;
            parent.Remove(treeInspectorView.ExposedPropertyListView);
            SubTreeExposedPropertyListView exposedPropertyListView  = new SubTreeExposedPropertyListView();
            treeInspectorView.ExposedPropertyListView = exposedPropertyListView;
            exposedPropertyListView.name = "property-list-view";
            parent.Add(exposedPropertyListView);
            treeInspectorView.Init(tree, false);
            return treeInspectorView;
        }
    }
}

#endif