#if UNITY_EDITOR
using System;
using UnityEngine.UIElements;
using UnityEditor;

namespace TreeDesigner.Editor
{
    public class DropArea
    {
        VisualElement m_target;
        public event Action<DragUpdatedEvent> onDragUpdatedEvent;
        public event Action<DragLeaveEvent> onDragLeaveEvent;
        public event Action<DragPerformEvent> onDragPerformEvent;
        public event Action<DragExitedEvent> onDragExitEvent;

        public void Init(VisualElement target)
        {
            m_target = target;
            m_target.AddToClassList("droparea");
            m_target.RegisterCallback<AttachToPanelEvent>(OnAttach);
            m_target.RegisterCallback<DetachFromPanelEvent>(OnDetach);
            m_target.RegisterCallback<DragEnterEvent>(OnDragEnterEvent);
            m_target.RegisterCallback<DragExitedEvent>(OnDragExitedEvent);
            m_target.RegisterCallback<DragLeaveEvent>(OnDragLeaveEvent);
            m_target.RegisterCallback<DragUpdatedEvent>(OnDragUpdatedEvent);
            m_target.RegisterCallback<DragPerformEvent>(OnDragPerformEvent);
        }
        public void Dispose()
        {
            m_target?.RemoveFromClassList("droparea");
            m_target?.UnregisterCallback<AttachToPanelEvent>(OnAttach);
            m_target?.UnregisterCallback<DetachFromPanelEvent>(OnDetach);
            m_target?.UnregisterCallback<DragEnterEvent>(OnDragEnterEvent);
            m_target?.UnregisterCallback<DragExitedEvent>(OnDragExitedEvent);
            m_target?.UnregisterCallback<DragLeaveEvent>(OnDragLeaveEvent);
            m_target?.UnregisterCallback<DragUpdatedEvent>(OnDragUpdatedEvent);
            m_target?.UnregisterCallback<DragPerformEvent>(OnDragPerformEvent);
            m_target = null;
            onDragPerformEvent = null;
            onDragUpdatedEvent = null;
        }

        void OnDragEnterEvent(DragEnterEvent e)
        {
            m_target?.AddToClassList("dragover");
        }
        void OnDragExitedEvent(DragExitedEvent e)
        {
            onDragExitEvent?.Invoke(e);
            m_target?.RemoveFromClassList("dragover");
        }
        void OnDragLeaveEvent(DragLeaveEvent e)
        {
            m_target?.RemoveFromClassList("dragover");
            onDragLeaveEvent?.Invoke(e);
        }
        void OnDragUpdatedEvent(DragUpdatedEvent e)
        {
            m_target?.AddToClassList("dragover");
            object draggedVisualElement = DragAndDrop.GetGenericData(typeof(IDragableVisualElement).ToString());
            if (draggedVisualElement != null && draggedVisualElement is IDragableVisualElement dragableVisualElement)
            {
                dragableVisualElement.StartDrag(m_target);
            }
            else if (DragAndDrop.objectReferences.Length == 1)
            {
                if (DragAndDrop.objectReferences[0] is IDragableObejct dragableObejct)
                {
                    dragableObejct.StartDrag(m_target);
                }
            }
            onDragUpdatedEvent?.Invoke(e);
        }
        void OnDragPerformEvent(DragPerformEvent e)
        {
            DragAndDrop.AcceptDrag();
            object draggedVisualElement = DragAndDrop.GetGenericData(typeof(IDragableVisualElement).ToString());
            if (draggedVisualElement != null && draggedVisualElement is IDragableVisualElement dragableVisualElement)
            {
                dragableVisualElement.PerformDrag(e, m_target);
                dragableVisualElement.StopDrag(m_target);
            }
            else if (DragAndDrop.objectReferences.Length == 1)
            {
                if (DragAndDrop.objectReferences[0] is IDragableObejct dragable)
                {
                    dragable.PerformDrag(e, m_target);
                    dragable.StopDrag(m_target);
                }
            }
            onDragPerformEvent?.Invoke(e);
        }
        void OnAttach(AttachToPanelEvent e)
        {
            e.destinationPanel.visualTree.RegisterCallback<DragExitedEvent>(OnDragExitedEvent);
        }
        void OnDetach(DetachFromPanelEvent e)
        {
            e.originPanel.visualTree.UnregisterCallback<DragExitedEvent>(OnDragExitedEvent);
        }
    }
}
#endif