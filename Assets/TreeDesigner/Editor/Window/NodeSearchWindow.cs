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
    public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        protected TreeEditorWindow editorWindow;
        protected TreeView treeView;
        protected Texture2D indentationIcon;

        protected List<string> pathes;
        protected List<Type> validTypes;

        public List<Type> ValidTypes => validTypes;

        public virtual void Init(TreeEditorWindow editorWindow, TreeView treeView)
        {
            this.editorWindow = editorWindow;
            this.treeView = treeView;
            validTypes = new List<Type>();

            //ÀıΩ¯
            indentationIcon = new Texture2D(1, 1);
            indentationIcon.SetPixel(0, 0, new Color(0, 0, 0, 0));
            indentationIcon.Apply();

            foreach (var item in editorWindow.NodePathStart)
            {
                FindTypes(item);
            }
        }

        public virtual List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            pathes = new List<string>();
            var tree = new List<SearchTreeEntry>();
            tree.Add(new SearchTreeGroupEntry(new GUIContent("Create Nodes")));

            foreach (var item in editorWindow.NodePathStart)
            {
                tree.Add(new SearchTreeGroupEntry(new GUIContent(item), 1));
                var baseTypes = FindTypes(item);
                baseTypes = baseTypes.OrderBy(s => GetNodePath(s)).ToList();
                foreach (var type in baseTypes)
                {
                    CreateEntry(type, item, ref tree);
                }
            }
            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            var windowRoot = editorWindow.rootVisualElement;
            var windowMousePosition = windowRoot.ChangeCoordinatesTo(windowRoot.parent, context.screenMousePosition - editorWindow.position.position);
            var graphMousePosition = treeView.contentContainer.WorldToLocal(windowMousePosition);
            treeView.CreateNode(SearchTreeEntry.userData as Type, graphMousePosition);
            return true;
        }

        protected string GetNodePath(Type type)
        {

            var nodePathes = type.GetCustomAttributes(typeof(NodePath), false);
            if (nodePathes.Length == 0)
                return string.Empty;
            var nodePath = nodePathes[0] as NodePath;
            
            ///¡Ÿ ±”√
            if (TreeDesignerSet.Instance.showChinese && TreeDesignerSet.Instance.NodeName(type) != string.Empty)
            {
                var pathStrs = nodePath.Path.Split(new char[] { '/' });
                pathStrs[pathStrs.Length - 1] = TreeDesignerSet.Instance.NodeName(type);
                var nodePathStr = string.Empty;
                for (int i = 0; i < pathStrs.Length; i++)
                {
                    nodePathStr += pathStrs[i];
                    if (i < pathStrs.Length - 1)
                        nodePathStr += "/";
                }
                return nodePathStr;
            }
            
            return nodePath.Path;
        }
        protected List<Type> FindTypes(string startWith)
        {
            List<Type> types = new List<Type>();
            var childTypes = TypeCache.GetTypesDerivedFrom<BaseNode>();
            foreach (var type in childTypes)
            {
                if (type.IsAbstract)
                    continue;
                var nodePathes = type.GetCustomAttributes(typeof(NodePath), false);
                if (nodePathes.Length == 0)
                    continue;
                var nodePath = nodePathes[0] as NodePath;
                if (!nodePath.Path.StartsWith(startWith))
                    continue;
                var pathStrs = nodePath.Path.Split(new char[] { '/' });
                if (pathStrs.Length == 1)
                    continue;
                types.Add(type);
            }
            foreach (var item in editorWindow.NodeTypeFilter)
            {
                if (types.Contains(item))
                    types.Remove(item);
            }
            validTypes.AddRange(types);
            return types;
        }
        protected void CreateEntry(Type type, string startWith, ref List<SearchTreeEntry> tree)
        {
            var pathStrs = GetNodePath(type).Split(new char[] { '/' });
            if (pathStrs.Length == 1)
                return;
            string path = $"{startWith}/";
            for (int i = 1; i < pathStrs.Length; i++)
            {
                int level = i + 1;
                string pathStr = pathStrs[i];
                if (i == pathStrs.Length - 1)
                {
                    tree.Add(new SearchTreeEntry(new GUIContent(pathStrs[i], indentationIcon))
                    {
                        userData = type,
                        level = level
                    });
                }
                else
                {
                    path += pathStr + "/";
                    if (!pathes.Contains(path))
                    {
                        tree.Add(new SearchTreeGroupEntry(new GUIContent(pathStr), level));
                        pathes.Add(path);
                    }
                }
            }
        }
    }
}
#endif