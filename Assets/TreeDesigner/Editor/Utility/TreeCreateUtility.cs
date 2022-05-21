#if UNITY_EDITOR
using System.IO;
using UnityEngine;
using UnityEditor;
using TreeDesigner.Runtime;

namespace TreeDesigner.Editor
{
    public class TreeCreateUtility
    {
        [MenuItem("Assets/Create/Tree/BaseTree")]
        public static void CreateBaseTree()
        {
            BaseTree tree = ScriptableObject.CreateInstance<BaseTree>();

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New Base Tree.asset");
            AssetDatabase.CreateAsset(tree, assetPathAndName);
            AssetDatabase.SaveAssets();

            AssetDatabase.Refresh();
            Selection.activeObject = tree;

            tree.rootNode = tree.CreateNode(typeof(RootNode));
            EditorUtility.SetDirty(tree);
        }

        [MenuItem("Assets/Create/Tree/SubTree")]
        public static void CreateSubTree()
        {
            SubTree tree = ScriptableObject.CreateInstance<SubTree>();


            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New Sub Tree.asset");
            AssetDatabase.CreateAsset(tree, assetPathAndName);
            AssetDatabase.SaveAssets();

            AssetDatabase.Refresh();
            Selection.activeObject = tree;
            EditorUtility.SetDirty(tree);
        }

        [MenuItem("Assets/Create/Tree/GlobalBoolProperty")]
        public static void CreateGlobalBoolProperty()
        {
            ExposedBoolProperty exposedProperty = ScriptableObject.CreateInstance<ExposedBoolProperty>();
            exposedProperty.Global = true;

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New ExposedBoolProperty.asset");
            AssetDatabase.CreateAsset(exposedProperty, assetPathAndName);
            AssetDatabase.SaveAssets();

            AssetDatabase.Refresh();
            Selection.activeObject = exposedProperty;
            EditorUtility.SetDirty(exposedProperty);

        }

        [MenuItem("Assets/Create/Tree/GlobalIntProperty")]
        public static void CreateGlobalIntProperty()
        {
            ExposedIntProperty exposedProperty = ScriptableObject.CreateInstance<ExposedIntProperty>();
            exposedProperty.Global = true;

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New ExposedBoolProperty.asset");
            AssetDatabase.CreateAsset(exposedProperty, assetPathAndName);
            AssetDatabase.SaveAssets();

            AssetDatabase.Refresh();
            Selection.activeObject = exposedProperty;
            EditorUtility.SetDirty(exposedProperty);

        }

        [MenuItem("Assets/Create/Tree/GlobalFloatProperty")]
        public static void CreateGlobalFloatProperty()
        {
            ExposedFloatProperty exposedProperty = ScriptableObject.CreateInstance<ExposedFloatProperty>();
            exposedProperty.Global = true;

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New ExposedBoolProperty.asset");
            AssetDatabase.CreateAsset(exposedProperty, assetPathAndName);
            AssetDatabase.SaveAssets();

            AssetDatabase.Refresh();
            Selection.activeObject = exposedProperty;
            EditorUtility.SetDirty(exposedProperty);

        }

        [MenuItem("Assets/Create/Tree/GlobalStringProperty")]
        public static void CreateGlobalStringProperty()
        {
            ExposedStringProperty exposedProperty = ScriptableObject.CreateInstance<ExposedStringProperty>();
            exposedProperty.Global = true;

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New ExposedBoolProperty.asset");
            AssetDatabase.CreateAsset(exposedProperty, assetPathAndName);
            AssetDatabase.SaveAssets();

            AssetDatabase.Refresh();
            Selection.activeObject = exposedProperty;
            EditorUtility.SetDirty(exposedProperty);

        }

        public static void CreateSubTree<T>(Object parent) where T : SubTree
        {
            SubTree tree = ScriptableObject.CreateInstance<T>();
            tree.parentTree = parent as BaseTree;
            tree.name = "New Sub Tree";
            if (!Application.isPlaying)
                AssetDatabase.AddObjectToAsset(tree, parent);
            AssetDatabase.SaveAssets();

            AssetDatabase.Refresh();
            Selection.activeObject = tree;
            EditorUtility.SetDirty(tree);
        }
        public static void CreateSubTree<T>(string path) where T : SubTree
        {
            SubTree tree = ScriptableObject.CreateInstance<T>();
            tree.name = "New Sub Tree";
            AssetDatabase.CreateAsset(tree, path);
            AssetDatabase.SaveAssets();

            AssetDatabase.Refresh();
            Selection.activeObject = tree;
            EditorUtility.SetDirty(tree);
        }

        //[MenuItem("Tools/TreeEditor/CheckRoot")]
        public static void CheckRoot()
        {
            string dirPath = AssetDatabase.GetAssetOrScenePath(Selection.activeObject);
            if (File.Exists(dirPath))
            {
                dirPath = dirPath.Substring(0, dirPath.LastIndexOf("/"));
            }
            var trees = Resources.LoadAll<BaseTree>(dirPath);
            foreach (var t in trees)
            {
                if(!t.rootNode)
                    Debug.Log($"<color=red>{t.treeName}</color> + {AssetDatabase.GetAssetPath(t)}");
                Resources.UnloadAsset(t);
            }
        }
        public void CheckNullNode()
        {

        }
    }
}
#endif