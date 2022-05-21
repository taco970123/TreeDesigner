#if UNITY_EDITOR
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TreeDesigner.Runtime;

namespace TreeDesigner.Editor
{
    public static class TreeDesignerUtility
    {
        static string path;
        public static string Path 
        {
            get
            {
                if (string.IsNullOrEmpty(path))
                {
                    string[] pathes = AssetDatabase.FindAssets(nameof(TreeDesignerUtility));
                    if(pathes.Length > 1)
                    {
                        Debug.Log("有重名文件");
                    }
                    else
                    {
                        path = AssetDatabase.GUIDToAssetPath(pathes[0]).Replace(@"/" + nameof(TreeDesignerUtility) + ".cs", "");
                        string[] pathSplits = path.Split('/');
                        path = string.Empty;
                        for (int i = 0; i < pathSplits.Length - 2; i++)
                        {
                            path += pathSplits[i] + "/";
                        }
                    }
                }
                return path;
            }
        }

        public static List<BaseNode> NodeBuffer;
        public static List<DescriptionNote> NoteBuffer;

        static Dictionary<Type, TreeEditorWindow> windows;
        static Type[] allWindowTypes;
        public static TreeEditorWindow GetWindow<T>() where T : TreeEditorWindow
        {
            if(windows == null)
            {
                windows = new Dictionary<Type, TreeEditorWindow>();
                var types = TypeCache.GetTypesDerivedFrom<TreeEditorWindow>().ToList();
                types = types.OrderByDescending(i => 
                {
                    int count = 0;
                    Type baseType = i.BaseType;
                    while (baseType != null)
                    {
                        count++;
                        baseType = baseType.BaseType;
                    }
                    return count;
                }).ToList();

                types.Add(typeof(TreeEditorWindow));
                allWindowTypes = types.ToArray();

                foreach (var item in types)
                {
                    TreeEditorWindow editorWindow = GetExistWindow(item);
                    if(editorWindow != null)
                        windows.Add(item, editorWindow);
                }
            }
            if (windows.ContainsKey(typeof(T)))
            {
                TreeEditorWindow editorWindow = windows[typeof(T)];
                if (editorWindow == null)
                {
                    editorWindow = EditorWindow.CreateWindow<T>(allWindowTypes);
                    windows[typeof(T)] = editorWindow;
                }
                editorWindow.titleContent = new GUIContent(typeof(T).Name);
                editorWindow.Show();
                editorWindow.Focus();
                return editorWindow;
            }
            else
            {
                TreeEditorWindow editorWindow = EditorWindow.CreateWindow<T>(allWindowTypes);
                editorWindow.titleContent = new GUIContent(typeof(T).Name);
                editorWindow.Show();
                editorWindow.Focus();
                windows.Add(typeof(T), editorWindow);
            }
            return null;
        }
        static TreeEditorWindow GetExistWindow(Type type)
        {
            UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(type);
            foreach (var item in array)
            {
                if (item.GetType() == type)
                    return item as TreeEditorWindow;
            }
            return null;
        }
        
        
        public static Action<BaseTree> onOtherTreeUpdate;
        public static Action<BaseNode> onNodeUpdate;

    }
}
#endif