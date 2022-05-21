#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.UIElements;

namespace TreeDesigner.Editor
{
    public class DragHandle
    {
        bool m_GotMouseDown;
        VisualElement m_dragItem;
        VisualElement m_dragTarget;

        public void Init(VisualElement target)
        {
            m_dragItem = target;
            m_dragTarget = target;
            m_dragItem.RegisterCallback<MouseDownEvent>(OnMouseDownEvent);
            m_dragItem.RegisterCallback<MouseMoveEvent>(OnMouseMoveEvent);
            m_dragItem.RegisterCallback<MouseUpEvent>(OnMouseUpEvent);
        }
        public void Init(VisualElement dragItem, VisualElement dragType)
        {
            Init(dragItem);
            m_dragTarget = dragType;
        }
        public void Dispose()
        {
            m_dragItem?.UnregisterCallback<MouseDownEvent>(OnMouseDownEvent);
            m_dragItem?.UnregisterCallback<MouseMoveEvent>(OnMouseMoveEvent);
            m_dragItem?.UnregisterCallback<MouseUpEvent>(OnMouseUpEvent);
        }

        void OnMouseDownEvent(MouseDownEvent e)
        {
            if (e.button == 0)
            {
                m_GotMouseDown = true;
            }
        }
        void OnMouseMoveEvent(MouseMoveEvent e)
        {
            if (m_GotMouseDown && e.pressedButtons == 1)
            {
                StartDraggingBox();
                m_GotMouseDown = false;
            }
        }
        void OnMouseUpEvent(MouseUpEvent e)
        {
            if (m_GotMouseDown && e.button == 0)
            {
                m_GotMouseDown = false;
            }
        }
        void StartDraggingBox()
        {
            DragAndDrop.PrepareStartDrag();
            DragAndDrop.SetGenericData(typeof(IDragableVisualElement).ToString(), m_dragTarget);
            DragAndDrop.StartDrag(typeof(IDragableVisualElement).ToString());
        }
    }
}
#endif