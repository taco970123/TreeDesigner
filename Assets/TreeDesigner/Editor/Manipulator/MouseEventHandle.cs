#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.Experimental.GraphView
{
    public class MouseEventHandle : MouseManipulator
    {
        Action<MouseDownEvent> onMouseDown;
        Action<MouseUpEvent> onMouseUp;
        Action<MouseMoveEvent> onMouseMove;

        public MouseEventHandle(Action<MouseDownEvent> onMouseDown, Action<MouseUpEvent> onMouseUp, Action<MouseMoveEvent> onMouseMove) 
        {
            activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
            if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
                activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse, modifiers = EventModifiers.Command });
            else
                activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse, modifiers = EventModifiers.Control });

            this.onMouseDown = onMouseDown;
            this.onMouseUp = onMouseUp;
            this.onMouseMove = onMouseMove;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            //var graphView = target as GraphView;

            //if (graphView == null)
            //{
            //    throw new InvalidOperationException("Manipulator can only be added to a GraphView");
            //}

            target.RegisterCallback<MouseDownEvent>(OnMouseDownCallback);
            target.RegisterCallback<MouseUpEvent>(OnMouseUpCallback);
            target.RegisterCallback<MouseMoveEvent>(OnMouseMoveCallback);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDownCallback);
            target.UnregisterCallback<MouseUpEvent>(OnMouseUpCallback);
            target.UnregisterCallback<MouseMoveEvent>(OnMouseMoveCallback);
        }

        void OnMouseDownCallback(MouseDownEvent e)
        {
            if(onMouseDown != null)
                onMouseDown.Invoke(e);
        }
        void OnMouseUpCallback(MouseUpEvent e)
        {
            if(onMouseUp != null)
                onMouseUp.Invoke(e);
        }
        void OnMouseMoveCallback(MouseMoveEvent e)
        {
            if(onMouseMove != null)
                onMouseMove.Invoke(e);
        }
    }
}

#endif