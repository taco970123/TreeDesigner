using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEngine.UIElements;
using UnityEditor;
using TreeDesigner.Editor;
#endif

namespace TreeDesigner.Runtime
{
    public class SubTree : BaseTree
#if UNITY_EDITOR
    , IDragableObejct
#endif
    {
        [HideInInspector]
        public SubTreeNode holder;
        [HideInInspector]
        public BaseNode endNode;

        [SerializeField, HideInInspector]
        List<ExposedProperty> outExposedProperties = new List<ExposedProperty>();
        [SerializeField, HideInInspector]
        List<ExposedProperty> inExposedProperties = new List<ExposedProperty>();

        public List<ExposedProperty> OutExposedProperties => outExposedProperties;
        public List<ExposedProperty> InExposedProperties => inExposedProperties;


        public override BaseTree Clone()
        {
            SubTree tree = Instantiate(this);
            tree.name = name;
            tree.nodes = new List<BaseNode>();
            Dictionary<BaseNode, BaseNode> cloneNodePairs = new Dictionary<BaseNode, BaseNode>();
            nodes.ForEach((i) =>
            {
                BaseNode cloneNode = i.Clone();
                cloneNode.Owner = tree;
                tree.nodes.Add(cloneNode);
                cloneNodePairs.Add(i, cloneNode);
            });
            if(rootNode)
                tree.rootNode = cloneNodePairs[rootNode];
            if(endNode)
                tree.endNode = cloneNodePairs[endNode];

            Dictionary<ExposedProperty, ExposedProperty> cloneExposedPropertyPair = new Dictionary<ExposedProperty, ExposedProperty>();
            for (int i = 0; i < exposedProperties.Count; i++)
            {
                ExposedProperty cloneExposedProperty = exposedProperties[i].Clone();
                cloneExposedPropertyPair.Add(exposedProperties[i], cloneExposedProperty);
                tree.exposedProperties[i] = cloneExposedProperty;
                int inIndex = inExposedProperties.IndexOf(exposedProperties[i]);
                if (inIndex != -1)
                    tree.inExposedProperties[inIndex] = cloneExposedProperty;
                int outIndex = outExposedProperties.IndexOf(exposedProperties[i]);
                if (outIndex != -1)
                    tree.outExposedProperties[outIndex] = cloneExposedProperty;
            }
            foreach (var cloneNodePair in cloneNodePairs)
            {
                foreach (var child in cloneNodePair.Key.GetChildren())
                {
                    cloneNodePair.Value.AddChild(cloneNodePairs[child]);
                }
                foreach (var linkData in cloneNodePair.Value.LinkDatas)
                {
                    linkData.sourceNode = cloneNodePairs[linkData.sourceNode];
                }
            }

            foreach (var item in tree.nodes)
            {
                item.UpdateSharedValue(cloneExposedPropertyPair);
                item.UpdateLinkValue();
            }
            tree.ResetState();
            return tree;
        }

#if UNITY_EDITOR
        [HideInInspector]
        public BaseTree parentTree;
        
        public override BaseNode CreateNode(Type type)
        {
            var node = CreateInstance(type) as BaseNode;
            node.name = type.Name;
            node.Enable = node.ParentEnable = true;
            node.GUID = GUID.Generate().ToString();
            node.hideFlags = HideFlags.HideInHierarchy;

            Undo.RecordObject(this, "Tree (Create Node)");
            nodes.Add(node);
            if (node is SubTreeEntryNode subTreeEntryNode)
            {
                rootNode = subTreeEntryNode;
                TreeDesignerUtility.onOtherTreeUpdate?.Invoke(this);
            }
            if (node is SubTreeExitNode subTreeExitNode)
            {
                endNode = subTreeExitNode;
                TreeDesignerUtility.onOtherTreeUpdate?.Invoke(this);
            }
            onNodeChangedEvent?.Invoke();
            if (!Application.isPlaying)
                AssetDatabase.AddObjectToAsset(node, this);
            Undo.RegisterCreatedObjectUndo(node, "Tree (Create Node)");

            AssetDatabase.SaveAssets();
            return node;
        }  
        public override void DeleteNode(BaseNode node)
        {
            Undo.RecordObject(this, "Tree (Delete Node)");
            nodes.Remove(node);
            if (node is SubTreeEntryNode subTreeEntryNode)
            {
                rootNode = null;
                TreeDesignerUtility.onOtherTreeUpdate?.Invoke(this);
            }
            if (node is SubTreeExitNode subTreeExitNode)
            {
                endNode = null;
                TreeDesignerUtility.onOtherTreeUpdate?.Invoke(this);
            }
            onNodeChangedEvent?.Invoke();
            Undo.DestroyObjectImmediate(node);
            AssetDatabase.SaveAssets();
        }

        public override void RemoveProperty(ExposedProperty exposedProperty)
        {
            base.RemoveProperty(exposedProperty);
            if (outExposedProperties.Contains(exposedProperty))
                RemoveOutProperty(exposedProperty);
            if (inExposedProperties.Contains(exposedProperty))
                RemoveInProperty(exposedProperty);
        }
        public void AddOutProperty(ExposedProperty exposedProperty)
        {
            Undo.RecordObject(this, "Tree (Add Out Property)");
            outExposedProperties.Add(exposedProperty);
            onExposedPropertyChangedEvent?.Invoke();
            TreeDesignerUtility.onOtherTreeUpdate?.Invoke(this);
            EditorUtility.SetDirty(this);
        }
        public void AddInProperty(ExposedProperty exposedProperty)
        {
            Undo.RecordObject(this, "Tree (Add In Property)");
            inExposedProperties.Add(exposedProperty);
            onExposedPropertyChangedEvent?.Invoke();
            TreeDesignerUtility.onOtherTreeUpdate?.Invoke(this);
            EditorUtility.SetDirty(this);
        }
        public void RemoveOutProperty(ExposedProperty exposedProperty)
        {
            Undo.RecordObject(this, "Tree (Remove Out Property)");
            outExposedProperties.Remove(exposedProperty);
            onExposedPropertyChangedEvent?.Invoke();
            TreeDesignerUtility.onOtherTreeUpdate?.Invoke(this);
            EditorUtility.SetDirty(this);
        }
        public void RemoveInProperty(ExposedProperty exposedProperty)
        {
            Undo.RecordObject(this, "Tree (Remove In Property)");
            inExposedProperties.Remove(exposedProperty);
            onExposedPropertyChangedEvent?.Invoke();
            TreeDesignerUtility.onOtherTreeUpdate?.Invoke(this);
            EditorUtility.SetDirty(this);
        }

        Vector2 localMousePosition;
        public void StartDrag(object dragArea) 
        {
            if (dragArea is TreeView treeView && !(treeView.Tree is SubTree))
            {
                if(parentTree == null || parentTree == treeView.Tree)
                    DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
            }
        }
        public void StopDrag(object dragArea)
        { 

        }
        public void PerformDrag(DragPerformEvent e, object dragArea)
        {
            if (dragArea is TreeView treeView && !(treeView.Tree is SubTree))
            {
                if (parentTree == null || parentTree == treeView.Tree)
                    treeView.panel.contextualMenuManager.DisplayMenu(e, treeView);
            }
            localMousePosition = e.localMousePosition;
        }
        public void BuildContextMenu(ContextualMenuPopulateEvent e, object dragArea)
        {
            if (dragArea is TreeView treeView)
            {
                e.menu.AppendAction("Create Sub Tree Node", (a) =>
                {
                    treeView.CreateNodeWithOutsideValue(typeof(SubTreeNode), localMousePosition, this);
                });
            }
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            
        }
        [ContextMenu("Delete")]
        void Delete()
        {
            DestroyImmediate(this, true);
            AssetDatabase.SaveAssets();
        }
#endif
    }
}
