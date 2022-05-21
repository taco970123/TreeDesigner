#if UNITY_EDITOR
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using TreeDesigner.Runtime;

namespace TreeDesigner.Editor
{
    public class TreeView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<TreeView, UxmlTraits> { }

        protected BaseTree tree;
        protected TreeEditorWindow treeEditor;
        protected NodeSearchWindow nodeSearchWindow;
        protected NodeInfoView nodeInfoView;
        protected DropArea dropArea;
        protected List<DescriptionNoteView> descriptionNoteViews = new List<DescriptionNoteView>();

        public TreeView()
        {
            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new CustomRectangleSelector());

            var styleSheet = Resources.Load<StyleSheet>("UIBuilder/TreeViewStyle");
            styleSheets.Add(styleSheet);

            Undo.undoRedoPerformed += OnUndoRedo;
            TreeDesignerUtility.onOtherTreeUpdate += UpdateView;
            TreeDesignerUtility.onNodeUpdate += UpdateView;

            RegisterCallback<MouseEnterEvent>((e) => ExposedProperty.exposedProperties = tree ? tree.exposedProperties :new List<ExposedProperty>());
        }

        public BaseTree Tree => tree;
        public NodeSearchWindow NodeSearchWindow => nodeSearchWindow;

        public Action<NodeView> onNodeSelected;
        public Action<NodeView> onNodeUnselected;

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();
            NodeView sourceView = startPort.node as NodeView;
            if (startPort == sourceView.output && !sourceView.node.CanAddChild)
                return compatiblePorts;

            compatiblePorts = ports.ToList().Where(endPort => endPort.direction != startPort.direction
                                                           && endPort.node != startPort.node
                                                           && (endPort.portType == startPort.portType
                                                              || startPort.portType.IsAssignableFrom(endPort.portType)
                                                              || endPort.portType.IsAssignableFrom(startPort.portType))).ToList();

            for (int i = compatiblePorts.Count - 1; i >= 0; i--)
            {
                NodeView targetView = compatiblePorts[i].node as NodeView;
                if (compatiblePorts[i] == targetView.output && !targetView.node.CanAddChild)
                {
                    compatiblePorts.RemoveAt(i);
                    continue;
                }
                if (startPort == sourceView.input || startPort == sourceView.output)
                {
                    if (compatiblePorts[i] != targetView.input && compatiblePorts[i] != targetView.output)
                    {
                        compatiblePorts.RemoveAt(i);
                        continue;
                    }
                }
                if (startPort != sourceView.input && startPort != sourceView.output)
                {
                    if (compatiblePorts[i] == targetView.input || compatiblePorts[i] == targetView.output)
                    {
                        compatiblePorts.RemoveAt(i);
                        continue;
                    }
                }
                if (sourceView.ValuePorts.ContainsKey(startPort) && startPort.direction == Direction.Output)
                {
                    List<BaseNode> sourceNodes = new List<BaseNode>();
                    BaseTree.TraverseLink(sourceView.node, (n) =>
                    {
                        sourceNodes.Add(n);
                    });
                    if (sourceNodes.Contains(targetView.node))
                    {
                        compatiblePorts.RemoveAt(i);
                        continue;
                    }
                }
                if (sourceView.ValuePorts.ContainsKey(startPort) && startPort.direction == Direction.Input)
                {
                    List<BaseNode> sourceNodes = new List<BaseNode>();
                    BaseTree.TraverseLink(targetView.node, (n) =>
                    {
                        sourceNodes.Add(n);
                    });

                    if (sourceNodes.Contains(sourceView.node))
                    {
                        compatiblePorts.RemoveAt(i);
                        continue;
                    }
                }
            }
            return compatiblePorts;
        }
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (tree == null)
                return;

            object draggedVisualElement = DragAndDrop.GetGenericData(typeof(IDragableVisualElement).ToString());
            if (draggedVisualElement != null && draggedVisualElement is IDragableVisualElement dragableVisualElement)
            {
                dragableVisualElement.BuildContextMenu(evt, this);
                DragAndDrop.SetGenericData(typeof(IDragableVisualElement).ToString(), null);
            }
            else if (DragAndDrop.objectReferences.Length == 1 && DragAndDrop.objectReferences[0] is IDragableObejct dragableObejct)
            {
                dragableObejct.BuildContextMenu(evt, this);
                DragAndDrop.objectReferences = null;
            }
            else
            {
                Vector2 localPosition = evt.localMousePosition;
                evt.menu.AppendAction("Create Description Note", (a) =>
                {
                    CreateNote(localPosition);
                });
                tree.BuildContextualMenu(evt);
            }
        }

        public virtual void Init(TreeEditorWindow treeEditor)
        {
            this.treeEditor = treeEditor;
            AddSearchWindow();

            dropArea?.Dispose();
            dropArea = new DropArea();
            dropArea.Init(this);
        }
        public virtual void Dispose()
        {
            Undo.undoRedoPerformed -= OnUndoRedo;
            TreeDesignerUtility.onOtherTreeUpdate -= UpdateView;
        }
        public virtual void PopulateView(BaseTree tree)
        {

            if (!tree)
                return;
            if (this.tree)
                this.tree.onUpdateEvent -= UpdateNodeState;
            this.tree = tree;
            this.tree.onUpdateEvent += UpdateNodeState;

            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements.ToList());
            descriptionNoteViews.Clear();
            graphViewChanged += OnGraphViewChanged;

            //Create node view
            tree.nodes.ForEach(i => CreateNodeView(i));

            //Create edge
            tree.nodes.ForEach(i => CreateEdge(i));

            tree.descriptionNotes.ForEach(i => CreateDescriptionView(i));

            nodeInfoView = new NodeInfoView($"{TreeDesignerUtility.Path}Editor/Resources/UIBuilder/NodeInfoView.uxml");
            AddElement(nodeInfoView);
        }
        public virtual void AddSearchWindow()
        {
            nodeSearchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
            nodeSearchWindow.Init(treeEditor, this);
            nodeCreationRequest = context => 
            {
                if (tree != null)
                    SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), nodeSearchWindow);
            };
        }
        public virtual void UpdateNodeState()
        {
            nodes.ForEach(n =>
            {
                if (n is NodeView)
                {
                    NodeView nodeView = n as NodeView;
                    nodeView.UpdateState();
                }
            });
        }

        public void UpdateView(BaseTree tree)
        {
            if (tree == this.tree)
                return;
            PopulateView(this.tree);
        }
        public void UpdateView(BaseNode baseNode)
        {
            if (tree && tree.nodes.Contains(baseNode))
                PopulateView(tree);
        }
        public void ClearView()
        {
            tree = null;
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements.ToList());
            descriptionNoteViews.Clear();
        }

        public BaseNode CreateNode(Type type, Vector2 position)
        {
            var localPosition = (position - new Vector2(viewTransform.position.x, viewTransform.position.y)) / scale;
            var node = tree.CreateNode(type);
            if (node == null)
                return null;
            node.Position = localPosition;
            CreateNodeView(node);
            return node;
        }
        public BaseNode CreateNodeWithOutsideValue(Type type, Vector2 position, object outsideValue)
        {
            var localPosition = (position - new Vector2(viewTransform.position.x, viewTransform.position.y)) / scale;
            var node = tree.CreateNode(type);
            if (node == null)
                return null;
            node.Position = localPosition;
            (node as IOutsideValueNode).OutsideValue = outsideValue;
            CreateNodeView(node);
            return node;
        }
        public DescriptionNote CreateNote(Vector2 position)
        {
            var localPosition = (position - new Vector2(viewTransform.position.x, viewTransform.position.y)) / scale;

            var note = tree.CreateDescriptionNote();
            if (note == null)
                return null;
            note.Position = new Rect(localPosition.x, localPosition.y, note.Position.width, note.Position.height);
            CreateDescriptionView(note);
            return note;
        }
        public void Copy(List<BaseNode> nodes, List<DescriptionNote> notes, Vector2 position)
        {
            Dictionary<BaseNode, BaseNode> oriCloneNodePair = new Dictionary<BaseNode, BaseNode>();
            Dictionary<DescriptionNote, DescriptionNote> oriCloneNotePair = new Dictionary<DescriptionNote, DescriptionNote>();
            Vector2 distance = Vector2.zero;
            if (nodes.Count != 0)
                distance = position - nodes[0].Position;
            else if(notes.Count != 0)
                distance = position - new Vector2(notes[0].Position.x, notes[0].Position.y);

            if (nodes.Count != 0)
            {
                foreach (var oriNode in nodes)
                {
                    var cloneNode = tree.CopyNode(oriNode);
                    if (cloneNode == null)
                        continue;
                    cloneNode.Position = oriNode.Position + distance;
                    oriCloneNodePair.Add(oriNode, cloneNode);
                }
                foreach (var node in nodes)
                {
                    List<BaseNode> children = node.GetChildren();
                    foreach (var child in children)
                    {
                        if (oriCloneNodePair.TryGetValue(child, out BaseNode cloneNode))
                            oriCloneNodePair[node].AddChild(cloneNode);
                    }
                    foreach (var linkData in node.LinkDatas)
                    {
                        if (oriCloneNodePair.TryGetValue(linkData.sourceNode, out BaseNode cloneSourceNode))
                            oriCloneNodePair[node].LinkDatas.Add(new NodeLinkData(cloneSourceNode, linkData.outputValueName, linkData.inputValueName));
                    }
                }
            }
            if (notes.Count != 0) 
            {
                foreach (var oriNote in notes)
                {
                    var cloneNote = tree.CopyDescriptionNote(oriNote);
                    if (cloneNote == null)
                        continue;
                    cloneNote.Position = new Rect(oriNote.Position.x + distance.x,
                                                  oriNote.Position.y + distance.y,
                                                  oriNote.Position.width,
                                                  oriNote.Position.height);
                    oriCloneNotePair.Add(oriNote, cloneNote);
                }
                foreach (var item in oriCloneNotePair)
                {
                    CreateDescriptionView(item.Value);
                }
            }

            selection.Clear();
            PopulateView(tree);
            foreach (var item in oriCloneNodePair)
            {
                NodeView nodeView = FindNodeView(item.Value);
                selection.Add(nodeView);
                nodeView.selected = true;
            }
            foreach (var item in oriCloneNotePair)
            {
                DescriptionNoteView descriptionNoteView = descriptionNoteViews.Find(i => i.note == item.Value);
                selection.Add(descriptionNoteView);
                descriptionNoteView.selected = true;
            }
        }


        protected virtual GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)
            {
                graphViewChange.elementsToRemove.ForEach(element =>
                {
                    if(element is NodeView nodeView)
                        tree.DeleteNode(nodeView.node);
                    if (element is DescriptionNoteView descriptionNoteView)
                        tree.DeleteDescriptionNote(descriptionNoteView.note);
                    if(element is Edge edge)
                    {
                        NodeView sourceView = edge.output.node as NodeView;
                        NodeView targetView = edge.input.node as NodeView;
                        if (sourceView.ValuePorts.ContainsKey(edge.output))
                        {
                            string inputName = targetView.ValuePorts[edge.input];
                            string outputName = sourceView.ValuePorts[edge.output];
                            tree.Unlink(sourceView.node, targetView.node, outputName, inputName);
                        }
                        else
                        {
                            tree.RemoveChild(sourceView.node, targetView.node);
                        }
                    }
                });
            }
            if (graphViewChange.edgesToCreate != null)
            {
                graphViewChange.edgesToCreate.ForEach(edge =>
                {
                    NodeView sourceView = edge.output.node as NodeView;
                    NodeView targetView = edge.input.node as NodeView;

                    string outputValueName = string.Empty;
                    if (sourceView.ValuePorts.TryGetValue(edge.output, out outputValueName))
                    {
                        string inputValueName = string.Empty;
                        if (targetView.ValuePorts.TryGetValue(edge.input, out inputValueName))
                        {
                            tree.Link(sourceView.node, targetView.node, outputValueName, inputValueName);
                        }
                    }
                    else
                    {
                        tree.AddChild(sourceView.node, targetView.node);
                        sourceView.SortChildren();
                    }
                });
            }
            if (graphViewChange.movedElements != null)
            {
                nodes.ForEach(n =>
                {
                    if (n is NodeView nodeView)
                    {
                        nodeView.SortChildren();
                        selection.Remove(nodeView);
                        nodeView.selected = false;
                    }
                });
            }
            return graphViewChange;
        }
        protected void OnUndoRedo()
        {
            if (tree == null)
                return;
            PopulateView(tree);
            AssetDatabase.SaveAssets();
        }

        protected void HideNodeInfo(NodeView nodeView)
        {
            nodeInfoView.Hide();
        }
        protected void ShowNodeInfo(NodeView nodeView)
        {
            nodeInfoView.Hide();
            nodeInfoView.Init(nodeView);
        }
    
        /// <summary>
        /// 选中所有子节点view
        /// </summary>
        /// <param name="nodeView"></param>
        protected void SelectChildren(NodeView nodeView)
        {
            selection.ForEach((i) =>
            {
                if (i is NodeView)
                    (i as NodeView).selected = false;
            });
            selection.Clear();

            List<NodeView> nodeViews = new List<NodeView>();
            Traverse(nodeView, (n) =>
            {
                nodeViews.Add(n);
            });
            foreach (var item in nodeViews)
            {
                selection.Add(item);
                item.selected = true;
            }
            HideNodeInfo(nodeView);
        }
        /// <summary>
        /// 对子节点view进行排序
        /// </summary>
        /// <param name="nodeView"></param>
        protected void OrderChildren(NodeView nodeView)
        {
            List<BaseNode> children = nodeView.node.GetChildren();
            if (children.Count == 0)
                return;
            for (int i = 1; i < children.Count; i++)
            {
                NodeView lowerNodeView = FindNodeView(children[i - 1]);
                children[i].Position = new Vector2(children[i].Position.x, lowerNodeView.node.Position.y + lowerNodeView.GetPosition().height + 25);
            }
            PopulateView(tree);
        }
        /// <summary>
        /// 遍历节点view的所有子节点view
        /// </summary>
        /// <param name="nodeView"></param>
        /// <param name="visiter"></param>
        protected void Traverse(NodeView nodeView, Action<NodeView> visiter)
        {
            if (nodeView != null)
            {
                visiter.Invoke(nodeView);
                var children = GetChildren(nodeView);
                children.ForEach((c) => Traverse(c, visiter));
            }
        }
        /// <summary>
        /// 得到节点view的所有子节点view
        /// </summary>
        /// <param name="baseNodeView"></param>
        /// <returns></returns>
        protected List<NodeView> GetChildren(NodeView baseNodeView)
        {
            List<NodeView> children = new List<NodeView>();
            List<BaseNode> childrenNodes = baseNodeView.node.GetChildren();
            if (childrenNodes != null)
            {
                foreach (var item in childrenNodes)
                {
                    children.Add(FindNodeView(item));
                }
            }
            return children;
        }
        /// <summary>
        /// 生成节点view
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected virtual NodeView CreateNodeView(BaseNode node)
        {
            NodeView nodeView = null;
            object[] nodeViewTypes = node.GetType().GetCustomAttributes(typeof(NodeViewType), true);
            if(nodeViewTypes.Length > 0)
            {
                NodeViewType nodeViewType = nodeViewTypes[0] as NodeViewType;
                nodeView = Activator.CreateInstance(nodeViewType.Type, node, $"{TreeDesignerUtility.Path}Editor/Resources/UIBuilder/{nodeViewType.Path}.uxml") as NodeView;
            }
            else
            {
                nodeView = new NodeView(node, $"{TreeDesignerUtility.Path}Editor/Resources/UIBuilder/NodeView.uxml");
            }
            nodeView.onNodeSelected = onNodeSelected;
            nodeView.onNodeUnselected = onNodeUnselected;
            nodeView.onNodeUnselected += HideNodeInfo;
            nodeView.onMouse0Down = HideNodeInfo;
            nodeView.onMouse2Down = SelectChildren;
            nodeView.onDoubleClicked = ShowNodeInfo;
            nodeView.onOrderChildren = OrderChildren;

            AddElement(nodeView);

            nodes.ForEach(n =>
            {
                if (n is NodeView nodeView)
                    nodeView.SortChildren();
            });
            return nodeView;
        }
        /// <summary>
        /// 找到对应view
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected virtual NodeView FindNodeView(BaseNode node)
        {
            return GetNodeByGuid(node.GUID) as NodeView;
        }
        /// <summary>
        /// 生成连线
        /// </summary>
        /// <param name="node"></param>
        protected virtual void CreateEdge(BaseNode node)
        {
            NodeView parentView = FindNodeView(node);
            List<BaseNode> invalidChild = new List<BaseNode>();
            var children = node.GetChildren();
            children.ForEach(child =>
            {
                NodeView childView = FindNodeView(child);
                if (parentView.output != null && childView.input != null)
                {
                    Edge edge = parentView.output.ConnectTo(childView.input);
                    AddElement(edge);
                }
                else
                    invalidChild.Add(child);
            });
            foreach (var item in invalidChild)
            {
                tree.RemoveChild(node, item);
            }

            NodeView targetView = FindNodeView(node);
            List<NodeLinkData> invalidLinkDatas = new List<NodeLinkData>();
            node.LinkDatas?.ForEach(linkData =>
            {
                Port outputPort = null;
                Port inputPort = null;
                NodeView sourceView = FindNodeView(linkData.sourceNode);
                if(sourceView != null)
                {
                    foreach (var item in sourceView.ValuePorts)
                    {
                        if (item.Value == linkData.outputValueName && item.Key.direction == Direction.Output)
                            outputPort = item.Key;
                    }
                }
                else
                {
                    invalidLinkDatas.Add(linkData);
                }
                foreach (var item in targetView.ValuePorts)
                {
                    if (item.Value == linkData.inputValueName && item.Key.direction == Direction.Input)
                        inputPort = item.Key;
                }
                if (outputPort == null || inputPort == null)
                {
                    invalidLinkDatas.Add(linkData);
                }
                else
                {
                    Edge edge = outputPort.ConnectTo(inputPort);
                    AddElement(edge);
                }
            });
            invalidLinkDatas.ForEach((linkData) =>
            {
                tree.Unlink(linkData.sourceNode, node, linkData.outputValueName, linkData.inputValueName);
            });
        }

        protected virtual void CreateDescriptionView(DescriptionNote descriptionNote)
        {
            var c = new DescriptionNoteView();
            c.Initialize(descriptionNote);
            AddElement(c);
            descriptionNoteViews.Add(c);
        }
    }
}
#endif