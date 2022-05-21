using UnityEngine;

namespace TreeDesigner.Runtime
{
    public class TreeRunner : MonoBehaviour
    {
        [SerializeField]
        BaseTree tree;

        public BaseTree Tree 
        {
            get => tree;
            set => tree = value;
        }

        void LateUpdate()
        {
            if (tree == null)
                return;
            if (tree.treeState == BaseNode.State.Running)
                tree.UpdateState();
        }
        [ContextMenu("StartTree")]
        public void StartTree()
        {
            tree.UpdateState();
        }
        [ContextMenu("ResetTree")]
        void ResetTree()
        {
            tree.ResetState();
        }
        [ContextMenu("CloneTree")]
        void CloneTree()
        {
            tree = tree?.Clone();
        }
    }
}
