#if UNITY_EDITOR
using UnityEngine.UIElements;

namespace TreeDesigner.Editor
{
    public interface IDragableObejct
    {
        public void StartDrag(object dragArea);
        public void StopDrag(object dragArea);
        public void PerformDrag(DragPerformEvent e, object dragArea);
        public void BuildContextMenu(ContextualMenuPopulateEvent e, object dragArea);
    }
}

#endif