#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace TreeDesigner.Editor
{
    public class CustomContextualMenuManipulator : MouseManipulator
    {
        private Action<ContextualMenuPopulateEvent> m_MenuBuilder;

        public CustomContextualMenuManipulator(Action<ContextualMenuPopulateEvent> menuBuilder)
        {
            m_MenuBuilder = menuBuilder;
            base.activators.Add(new ManipulatorActivationFilter
            {
                button = MouseButton.LeftMouse
            });
            //if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
            //{
            //    base.activators.Add(new ManipulatorActivationFilter
            //    {
            //        button = MouseButton.LeftMouse,
            //        modifiers = EventModifiers.Control
            //    });
            //}
        }

        //
        // ժҪ:
        //     Register the event callbacks on the manipulator target.
        protected override void RegisterCallbacksOnTarget()
        {
            if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
            {
                base.target.RegisterCallback<MouseDownEvent>(OnMouseUpDownEvent);
            }
            else
            {
                base.target.RegisterCallback<MouseUpEvent>(OnMouseUpDownEvent);
            }

            base.target.RegisterCallback<KeyUpEvent>(OnKeyUpEvent);
            base.target.RegisterCallback<ContextualMenuPopulateEvent>(OnContextualMenuEvent);
        }

        //
        // ժҪ:
        //     Unregister the event callbacks from the manipulator target.
        protected override void UnregisterCallbacksFromTarget()
        {
            if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
            {
                base.target.UnregisterCallback<MouseDownEvent>(OnMouseUpDownEvent);
            }
            else
            {
                base.target.UnregisterCallback<MouseUpEvent>(OnMouseUpDownEvent);
            }

            base.target.UnregisterCallback<KeyUpEvent>(OnKeyUpEvent);
            base.target.UnregisterCallback<ContextualMenuPopulateEvent>(OnContextualMenuEvent);
        }

        private void OnMouseUpDownEvent(IMouseEvent evt)
        {
            if (CanStartManipulation(evt) && base.target.panel != null && base.target.panel.contextualMenuManager != null)
            {
                EventBase eventBase = evt as EventBase;
                base.target.panel.contextualMenuManager.DisplayMenu(eventBase, base.target);
                eventBase.StopPropagation();
                eventBase.PreventDefault();
            }
        }

        private void OnKeyUpEvent(KeyUpEvent evt)
        {
            if (evt.keyCode == KeyCode.Menu && base.target.panel != null && base.target.panel.contextualMenuManager != null)
            {
                base.target.panel.contextualMenuManager.DisplayMenu(evt, base.target);
                evt.StopPropagation();
                evt.PreventDefault();
            }
        }

        private void OnContextualMenuEvent(ContextualMenuPopulateEvent evt)
        {
            if (m_MenuBuilder != null)
            {
                m_MenuBuilder(evt);
            }
        }
    }
}

#endif