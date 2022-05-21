#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Callbacks;
using TreeDesigner.Runtime;

namespace TreeDesigner.Editor
{
    public class TreeEditorWindow : EditorWindow
    {
        protected BaseTree tree;

        protected ScrollView treeFieldContent;
        protected TreeInspectorView treeInspectorContainer;
        protected Label treeNameLabel;
        protected TreeView treeView;
        protected Dictionary<BaseTree, TreeFieldView> treeFieldViews = new Dictionary<BaseTree, TreeFieldView>();
        protected List<BaseTree> editorModSelectTrees = new List<BaseTree>();

        public virtual List<string> NodePathStart => new List<string> { "Base" };
        public virtual List<Type> NodeTypeFilter => new List<Type>();

        protected virtual void OnEnable()
        {
        }
        protected virtual void OnDisable()
        {
            Undo.ClearAll();
            treeView?.Dispose();
            treeInspectorContainer?.ExposedPropertyListView.Dispose();
        }
        private void OnDestroy()
        {
            editorModSelectTrees.Clear();
        }


        protected virtual void Update()
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

            if (currentTree == null || tree == currentTree || currentTree.GetType() != typeof(BaseTree) || !AssetDatabase.CanOpenForEdit(currentTree) || (AssetDatabase.IsOpenForEdit(currentTree) && currentTree.name == string.Empty))
                return;
            if (!Application.isPlaying && currentTree.ClearInvalid())
                return;

            tree = currentTree;
            AddTree(currentTree);
            PopulateView();
        }

        public virtual void CreateGUI()
        {
            tree = null;
            VisualElement root = rootVisualElement;
            var visualTree = Resources.Load<VisualTreeAsset>("UIBuilder/TreeEditorWindow");
            visualTree.CloneTree(root);

            root.name = "TreeEditorWindow";
            root.RegisterCallback<KeyDownEvent>(KeyDownCallback);

            treeFieldContent = root.Q<ScrollView>("tree-field-content");
            Button clearViewBtn = root.Q<Button>("clear-view-btn");
            clearViewBtn.clicked += () => ClearTree();

            treeInspectorContainer = root.Q<TreeInspectorView>("tree-inspector-container");
            OnCreateTreeInspector();

            treeNameLabel = root.Q<Label>("tree-name");
            treeNameLabel.bindingPath = nameof(tree.treeName);

            treeView = root.Q<TreeView>("tree-view");
            OnCreateTreeView();
            treeView.Init(this);
            treeView.onNodeSelected = OnNodeSelectionChanged;
            treeView.onNodeUnselected = OnNodeUnselected;

            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        protected virtual void OnCreateTreeInspector()
        {

        }
        protected virtual void OnCreateTreeView()
        {
            
        }

        [MenuItem("Tools/TreeEditor/TreeEditorWindow")]
        public static void OpenWindow()
        {
            TreeDesignerUtility.GetWindow<TreeEditorWindow>();
        }
        [OnOpenAsset(0)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            if (!Application.isPlaying && Selection.activeObject != null && Selection.activeObject.GetType() == typeof(BaseTree))
                OpenWindow();
            return false;
        }


        protected virtual void AddTree(BaseTree tree)
        {
            if (tree == null)
                return;
            if (treeFieldViews.ContainsKey(tree))
                return;

            TreeFieldView treeFieldView = new TreeFieldView();
            treeFieldView.Init(tree);
            treeFieldView.onSelectedEvent = SelectTree;
            treeFieldView.onRemovedEvent = RemoveTree;
            treeFieldViews.Add(tree, treeFieldView);
            treeFieldContent.Add(treeFieldView);
            treeFieldContent.style.display = DisplayStyle.Flex;

            if(!Application.isPlaying && !editorModSelectTrees.Contains(tree))
                editorModSelectTrees.Add(tree);


        }
        protected virtual void RemoveTree(BaseTree tree)
        {
            if (tree == null)
                return;
            if (!treeFieldViews.ContainsKey(tree))
                return;

            treeFieldContent.Remove(treeFieldViews[tree]);
            treeFieldViews.Remove(tree);
            if (this.tree == tree)
                ClearTree();
            if (treeFieldContent.childCount == 0)
                treeFieldContent.style.display = DisplayStyle.None;

            if (!Application.isPlaying && editorModSelectTrees.Contains(tree))
                editorModSelectTrees.Remove(tree);
        }
        protected virtual void SelectTree(BaseTree tree)
        {
            this.tree = tree;
            Selection.activeObject = tree;
            PopulateView();
        }

        protected virtual void ClearTree()
        {
            tree = null;
            if (Selection.activeObject?.GetType() == typeof(BaseTree))
                Selection.activeObject = null;
            else if(Selection.activeGameObject?.GetComponent<TreeRunner>()?.Tree?.GetType() == typeof(BaseTree))
                Selection.activeGameObject = null;
            ClearView();
        }
        protected virtual void PopulateView()
        {
            foreach (var item in treeFieldViews)
            {
                if (item.Key == tree)
                    item.Value.AddToClassList("selected");
                else
                    item.Value.RemoveFromClassList("selected");
            }

            treeNameLabel.Unbind();
            treeNameLabel.Bind(new SerializedObject(tree));
            treeNameLabel.text = tree.treeName;

            treeInspectorContainer.Init(tree, true);
            treeInspectorContainer.style.display = DisplayStyle.Flex;

            treeView.PopulateView(tree);
        }
        protected virtual void ClearView()
        {
            if (treeView == null)
                return;

            foreach (var item in treeFieldViews)
            {
                item.Value.RemoveFromClassList("selected");
            }

            treeNameLabel.Unbind();
            treeNameLabel.text = string.Empty;
            treeInspectorContainer.ClearView();
            treeInspectorContainer.style.display = DisplayStyle.None;
            treeView.ClearView();
        }

        public virtual void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {

        }

        protected void OnNodeSelectionChanged(NodeView nodeView)
        {

        }
        protected void OnNodeUnselected(NodeView nodeView)
        {

        }
        protected void OnPlayModeStateChanged(PlayModeStateChange obj)
        {
            switch (obj)
            {
                case PlayModeStateChange.EnteredEditMode:
                    editorModSelectTrees.ForEach(i => AddTree(i));
                    break;
                case PlayModeStateChange.ExitingEditMode:

                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
                default:
                    break;
            }
        }

        protected void KeyDownCallback(KeyDownEvent evt)
        {
            var windowRoot = rootVisualElement;
            var windowMousePosition = windowRoot.ChangeCoordinatesTo(windowRoot, evt.originalMousePosition);
            var graphMousePosition = treeView.contentViewContainer.WorldToLocal(windowMousePosition);

            if (evt.ctrlKey && evt.keyCode == KeyCode.C)
                Copy();
            if (evt.ctrlKey && evt.keyCode == KeyCode.V)
                Paste(graphMousePosition);
            if (evt.ctrlKey && evt.keyCode == KeyCode.D)
                Duplicate(graphMousePosition);
        }

        public void Copy()
        {
            List<BaseNode> selectedNodes = new List<BaseNode>();
            List<DescriptionNote> selectedNotes = new List<DescriptionNote>();
            foreach (var item in treeView.selection)
            {
                if (item is NodeView nodeView && nodeView.node.IsCopiable)
                    selectedNodes.Add(nodeView.node);
                if(item is DescriptionNoteView descriptionNoteView)
                    selectedNotes.Add(descriptionNoteView.note);
            }
            TreeDesignerUtility.NodeBuffer = selectedNodes;
            TreeDesignerUtility.NoteBuffer = selectedNotes;
        }
        public void Paste(Vector2 position)
        {
            List<BaseNode> validNodes = TreeDesignerUtility.NodeBuffer.Where(n => n && treeView.NodeSearchWindow.ValidTypes.Contains(n.GetType())).ToList();
            List<DescriptionNote> validNotes = TreeDesignerUtility.NoteBuffer.Where(n => n).ToList();
            treeView.Copy(validNodes, validNotes, position);
        }
        public void Duplicate(Vector2 position)
        {
            if (treeView.selection.Count != 0)
                Copy();
            Paste(position);
        }
    }
}
#endif