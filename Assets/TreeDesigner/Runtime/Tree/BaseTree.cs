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
    public class BaseTree : ScriptableObject
    {
        public string treeName;
        [HideInInspector]
        public BaseNode rootNode;
        [HideInInspector]
        public BaseNode.State treeState;
        [HideInInspector]
        public List<BaseNode> nodes = new List<BaseNode>();
        [HideInInspector]
        public List<ExposedProperty> exposedProperties = new List<ExposedProperty>();

        bool running;
        public Action onUpdateEvent;
        public Action onStoppedEvent;

        Dictionary<ExposedProperty, object> exposedPropertyValuePair = new Dictionary<ExposedProperty, object>(); 


        public bool Running => running;

        public virtual void ResetState()
        {
            running = false;
            treeState = BaseNode.State.Default;
            nodes.ForEach((i) => i.ResetState());
            onUpdateEvent?.Invoke();
        }
        public virtual BaseNode.State UpdateState()
        {
            if (running == false)
                OnStarted();
            if (treeState == BaseNode.State.Running)
            {
                treeState = rootNode.UpdateState();
                onUpdateEvent?.Invoke();
            }
            if (treeState == BaseNode.State.Success || treeState == BaseNode.State.Failure)
                OnStopped();
            return treeState;
        }

        public virtual BaseTree Clone()
        {
            BaseTree tree = Instantiate(this);
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
            tree.rootNode = cloneNodePairs[rootNode];
            Dictionary<ExposedProperty, ExposedProperty> cloneExposedPropertyPair = new Dictionary<ExposedProperty, ExposedProperty>();
            for (int i = 0; i < exposedProperties.Count; i++)
            {
                ExposedProperty cloneExposedProperty = exposedProperties[i].Clone();
                cloneExposedPropertyPair.Add(exposedProperties[i], cloneExposedProperty);
                exposedPropertyValuePair.Add(cloneExposedProperty, cloneExposedProperty.GetValue());
                tree.exposedProperties[i] = cloneExposedProperty;
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
        public virtual void OnUnspawn()
        {
            foreach (var item in exposedPropertyValuePair)
            {
                item.Key.SetValue(item.Value);
            }
        }

        void Traverse(BaseNode node, Action<BaseNode> visiter)
        {
            if (node)
            {
                visiter.Invoke(node);
                var children = node.GetChildren();
                children.ForEach((c) => Traverse(c, visiter));
            }
        }
        public virtual void OnStarted()
        {
            ResetState();
            treeState = BaseNode.State.Running;
            running = true;
        }
        public virtual void OnStopped()
        {
            running = false;
            onStoppedEvent?.Invoke();
            onStoppedEvent = null;
        }

        public virtual object GetValue(string name)
        {
            ExposedProperty exposedProperty = exposedProperties.Find(i => i.Name == name);
            if (exposedProperty != null)
                return exposedProperty.GetValue();
            return null;
        }
        public virtual void SetValue(string name,object value)
        {
            ExposedProperty exposedProperty = exposedProperties.Find(i => i.Name == name);
            if (exposedProperty != null)
                exposedProperty.SetValue(value);
        }

#if UNITY_EDITOR
        [HideInInspector]
        public List<DescriptionNote> descriptionNotes = new List<DescriptionNote>();

        public virtual BaseNode CreateNode(Type type)
        {
            var node = CreateInstance(type) as BaseNode;
            node.name = type.Name;
            node.Enable = node.ParentEnable = true;
            node.GUID = GUID.Generate().ToString();
            node.hideFlags = HideFlags.HideInHierarchy;

            Undo.RecordObject(this, "Tree (Create Node)");
            nodes.Add(node);
            onNodeChangedEvent?.Invoke();
            if (!Application.isPlaying)
                AssetDatabase.AddObjectToAsset(node, this);
            Undo.RegisterCreatedObjectUndo(node, "Tree (Create Node)");

            AssetDatabase.SaveAssets();
            return node;
        }
        public virtual BaseNode CopyNode(BaseNode originalNode)
        {
            var copyNode = CreateInstance(originalNode.GetType()) as BaseNode;
            Type ori = originalNode.GetType();
            Type copy = copyNode.GetType();

            foreach (var tpi in ori.GetFields())
            {
                foreach (var ttpi in copy.GetFields())
                {
                    if (ttpi.Name == tpi.Name)
                        ttpi.SetValue(copyNode, tpi.GetValue(originalNode));
                }
            }
            copyNode.name = originalNode.name;
            copyNode.Enable = copyNode.ParentEnable = true;
            copyNode.InputPortDatas.AddRange(originalNode.InputPortDatas);
            copyNode.OutputPortDatas.AddRange(originalNode.OutputPortDatas);

            copyNode.GUID = GUID.Generate().ToString();
            copyNode.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;

            Undo.RecordObject(this, "Tree (Copy)");
            nodes.Add(copyNode);
            onNodeChangedEvent?.Invoke();
            if (!Application.isPlaying)
                AssetDatabase.AddObjectToAsset(copyNode, this);
            Undo.RegisterCreatedObjectUndo(copyNode, "Tree (Copy)");
            AssetDatabase.SaveAssets();
            return copyNode;
        }
        public virtual void DeleteNode(BaseNode node)
        {
            Undo.RecordObject(this, "Tree (Delete Node)");
            nodes.Remove(node);
            onNodeChangedEvent?.Invoke();
            Undo.DestroyObjectImmediate(node);
            AssetDatabase.SaveAssets();
        }

        public DescriptionNote CreateDescriptionNote()
        {
            var note = CreateInstance<DescriptionNote>();
            note.name = "DescriptionNote";
            note.title = "Title";
            note.content = "Content";
            note.Position = new Rect(0, 0, 200, 300);
            note.hideFlags = HideFlags.HideInHierarchy;
            Undo.RecordObject(this, "Tree (Create Note)");
            descriptionNotes.Add(note);
            if (!Application.isPlaying)
                AssetDatabase.AddObjectToAsset(note, this);
            Undo.RegisterCreatedObjectUndo(note, "Tree (Create Node)");
            AssetDatabase.SaveAssets();
            return note;
        }
        public DescriptionNote CopyDescriptionNote(DescriptionNote originalNote)
        {
            DescriptionNote copyNote = Instantiate(originalNote);
            copyNote.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            Undo.RecordObject(this, "Tree (Copy)");
            descriptionNotes.Add(copyNote);
            if (!Application.isPlaying)
                AssetDatabase.AddObjectToAsset(copyNote, this);
            Undo.RegisterCreatedObjectUndo(copyNote, "Tree (Copy)");
            AssetDatabase.SaveAssets();
            return copyNote;
        }
        public void DeleteDescriptionNote(DescriptionNote note)
        {
            Undo.RecordObject(this, "Tree (Delete Note)");
            descriptionNotes.Remove(note);
            Undo.DestroyObjectImmediate(note);
            AssetDatabase.SaveAssets();
        }

        public virtual void AddChild(BaseNode parent, BaseNode child)
        {
            Undo.RecordObject(parent, "Tree (Add Child)");
            parent.AddChild(child);
            EditorUtility.SetDirty(parent);
        }
        public virtual void RemoveChild(BaseNode parent, BaseNode child)
        {
            Undo.RecordObject(parent, "Tree (Remove Child)");
            parent.RemoveChild(child);
            EditorUtility.SetDirty(parent);
        }
        public virtual void Link(BaseNode sourceNode, BaseNode targetNode, string outputValueName, string inputValueName)
        {
            Undo.RecordObject(targetNode, "Tree (Link Node)");
            targetNode.Link(sourceNode, outputValueName, inputValueName);
            EditorUtility.SetDirty(targetNode);
        }
        public virtual void Unlink(BaseNode sourceNode, BaseNode targetNode, string outputValueName, string inputValueName)
        {
            Undo.RecordObject(targetNode, "Tree (Unlink Node)");
            targetNode.UnLink(sourceNode, outputValueName, inputValueName);
            EditorUtility.SetDirty(targetNode);
        }
        public static void TraverseLink(BaseNode targetNode, Action<BaseNode> visiter)
        {
            if (targetNode)
            {
                visiter.Invoke(targetNode);
                foreach (var item in targetNode.LinkDatas)
                {
                    TraverseLink(item.sourceNode, visiter);
                }
            }
        }

        public virtual ExposedProperty AddProperty(ExposedProperty exposedProperty, string exposedPropertyName)
        {
            exposedProperty.hideFlags = HideFlags.HideInHierarchy;
            Undo.RecordObject(this, "Tree (Create Property)");
            exposedProperties.Add(exposedProperty);
            onExposedPropertyChangedEvent?.Invoke();
            if (!Application.isPlaying)
                AssetDatabase.AddObjectToAsset(exposedProperty, this);
            Undo.RegisterCreatedObjectUndo(exposedProperty, "Tree (Create Property)");
            AssetDatabase.SaveAssets();
            return exposedProperty;
        }
        public virtual void RemoveProperty(ExposedProperty exposedProperty)
        {
            Undo.RecordObject(this, "Tree (Delete Property)");
            exposedProperties.Remove(exposedProperty);
            onExposedPropertyChangedEvent?.Invoke();
            Undo.DestroyObjectImmediate(exposedProperty);
            AssetDatabase.SaveAssets();
        }
        public void MoveProperty(int index, ExposedProperty exposedProperty)
        {
            Undo.RecordObject(this, "Tree (Move Property)");
            exposedProperties.Remove(exposedProperty);
            exposedProperties.Insert(index, exposedProperty);
            onExposedPropertyChangedEvent?.Invoke();
            EditorUtility.SetDirty(this);
        }


        public virtual bool ClearInvalid()
        {
            bool invalid = false;
            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                if (nodes[i] == null)
                {
                    invalid = true;
                    nodes.RemoveAt(i);
                }
                else
                {
                    List<BaseNode> children = nodes[i].GetChildren();
                    if (children != null && children.Count > 0)
                    {
                        for (int j = children.Count - 1; j >= 0; j--)
                        {
                            if (children[j] == null)
                                children.RemoveAt(j);
                        }
                    }
                    List<NodeLinkData> nodeLinkDatas = nodes[i].LinkDatas;
                    if (nodeLinkDatas != null && nodeLinkDatas.Count > 0)
                    {
                        for (int j = nodeLinkDatas.Count - 1; j >= 0; j--)
                        {
                            if (nodeLinkDatas[j].sourceNode == null)
                                nodeLinkDatas.RemoveAt(j);
                        }
                    }
                }
            }

            if (invalid)
            {
                string name = this.name;
                string oldPath = AssetDatabase.GetAssetPath(this);
                string newPath = oldPath.Replace(name, name + "(1)");
                var objects = AssetDatabase.LoadAllAssetsAtPath(oldPath);
                List<UnityEngine.Object> validObjects = new List<UnityEngine.Object>();
                for (int i = objects.Length - 1; i >= 0; i--)
                {
                    if (objects[i] != null)
                    {
                        if (objects[i] is BaseNode baseNode && nodes.Contains(baseNode))
                            validObjects.Add(baseNode);
                        else if (objects[i] is ExposedProperty exposedProperty && exposedProperties.Contains(exposedProperty))
                            validObjects.Add(exposedProperty);
                    }
                }
                AssetDatabase.RemoveObjectFromAsset(this);
                AssetDatabase.CreateAsset(this, newPath);
                AssetDatabase.SaveAssets();
                foreach (var item in validObjects)
                {
                    AssetDatabase.RemoveObjectFromAsset(item);
                    AssetDatabase.AddObjectToAsset(item, newPath);
                }
                AssetDatabase.MoveAssetToTrash(oldPath);

                foreach (var item in nodes)
                {
                    item.name = item.GetType().ToString();
                }
                AssetDatabase.MoveAsset(newPath, oldPath);
                AssetDatabase.SaveAssets();
            }
            return invalid;
        }

        public virtual void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Create Inside Sub Tree", (a) =>
            {
                TreeCreateUtility.CreateSubTree<SubTree>(this);

            });
            evt.menu.AppendAction("Create Outside Sub Tree", (a) =>
            {
                string path = EditorUtility.SaveFilePanelInProject(
                                    "Create Sub Tree",
                                    "New Sub Tree",
                                    "asset",
                                    "");
                TreeCreateUtility.CreateSubTree<SubTree>(path);
            });
        }
        [ContextMenu("ShowAllAssets")]
        void ShowAllAssets()
        {
            var objects = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this));
            foreach (var item in objects)
            {
                item.hideFlags = HideFlags.None;
            }
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
        [ContextMenu("HideAllAssets")]
        void HideAllAssets()
        {
            var objects = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this));
            foreach (var item in objects)
            {
                if (item == this)
                    continue;
                item.hideFlags = HideFlags.HideInHierarchy;
            }
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
        [ContextMenu("ClearPorts")]
        void ClearPorts()
        {
            nodes.ForEach(i => i.ClearPorts());
        }

        public Action onNodeChangedEvent;
        public Action onExposedPropertyChangedEvent;
#endif
    }
}
