#if UNITY_EDITOR
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Callbacks;
using TreeDesigner.Runtime;
using System;

namespace TreeDesigner.Editor
{
    public class SubTreeEditorWindow : TreeEditorWindow
    {
        public override List<string> NodePathStart => new List<string> { "Base" ,"Sub"};
        public override List<Type> NodeTypeFilter 
        {
            get
            {
                List<Type> filter = new List<Type>();
                if (tree && tree.rootNode)
                    filter.Add(typeof(SubTreeEntryNode));
                if (tree && (tree as SubTree).endNode)
                    filter.Add(typeof(SubTreeExitNode));
                return filter;
            }
        }

        protected override void Update()
        {
            if (treeView == null)
                return;
            if (tree == null)
            {
                ClearView();
                foreach (var item in treeFieldViews)
                {
                    if (item.Key == null)
                        treeFieldContent.Remove(item.Value);
                }
                treeFieldViews = treeFieldViews.Where(i => i.Key != null).ToDictionary(item => item.Key, item => item.Value);
            }

            BaseTree currentTree = null;
            if (Application.isPlaying)
            {
                currentTree = Selection.activeObject as BaseTree;
                string path = AssetDatabase.GetAssetPath(currentTree);
                if (path != string.Empty)
                    currentTree = null;
                if (currentTree == null && Selection.activeGameObject)
                {
                    TreeRunner treeRunner = Selection.activeGameObject.GetComponent<TreeRunner>();
                    if (treeRunner != null)
                        currentTree = treeRunner.Tree;
                }
            }
            else if (!Application.isPlaying)
            {
                currentTree = Selection.activeObject as BaseTree;
                if (currentTree == null && Selection.activeGameObject)
                {
                    TreeRunner treeRunner = Selection.activeGameObject.GetComponent<TreeRunner>();
                    if (treeRunner != null)
                        currentTree = treeRunner.Tree;
                }
            }
            if (currentTree == null || tree == currentTree || currentTree.GetType() != typeof(SubTree) || !AssetDatabase.CanOpenForEdit(currentTree) || (AssetDatabase.IsOpenForEdit(currentTree) && currentTree.name == string.Empty))
                return;
            if (!Application.isPlaying && currentTree.ClearInvalid())
                return;

            tree = currentTree;
            AddTree(currentTree);
            PopulateView();
        }

        protected override void OnCreateTreeInspector()
        {
            var parent = treeInspectorContainer.ExposedPropertyListView.parent;
            parent.Remove(treeInspectorContainer.ExposedPropertyListView);
            SubTreeExposedPropertyListView exposedPropertyListView = new SubTreeExposedPropertyListView();
            treeInspectorContainer.ExposedPropertyListView = exposedPropertyListView;
            exposedPropertyListView.name = "property-list-view";
            parent.Add(exposedPropertyListView);
        }


        [MenuItem("Tools/TreeEditor/SubTreeEditor")]
        public static new void OpenWindow()
        {
            TreeDesignerUtility.GetWindow<SubTreeEditorWindow>();
        }
        [OnOpenAsset(0)]
        public static new bool OnOpenAsset(int instanceID, int line)
        {
            if (!Application.isPlaying && Selection.activeObject != null && Selection.activeObject.GetType() == typeof(SubTree))
                OpenWindow();
            return false;
        }

        protected override void ClearTree()
        {
            tree = null;
            if (Selection.activeObject?.GetType() == typeof(SubTree))
                Selection.activeObject = null;
            else if (Selection.activeGameObject?.GetComponent<TreeRunner>()?.Tree?.GetType() == typeof(SubTree))
                Selection.activeGameObject = null;
            ClearView();
        }
    }
}

#endif