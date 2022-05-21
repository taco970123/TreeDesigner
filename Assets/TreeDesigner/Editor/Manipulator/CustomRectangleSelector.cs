#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.Experimental.GraphView
{
    public class CustomRectangleSelector : MouseManipulator
    {
        private readonly RectangleSelect m_Rectangle;
        bool m_Active;
        Action<MouseDownEvent> downCb;
        Action<MouseMoveEvent> moveCb;
        Action<MouseUpEvent> upCb;

        public CustomRectangleSelector() : this(new Vector2(-3.5f, 0), new Color(1.0f, 0.6f, 0.0f, 1.0f))
        {

        }
        public CustomRectangleSelector(Action<MouseDownEvent> downCb, Action<MouseMoveEvent> moveCb, Action<MouseUpEvent> upCb):this()
        {
            if (downCb != null)
            {
                this.downCb += downCb;

            }
            if (upCb != null)
            {
                this.upCb += upCb;
            }
            if (moveCb != null)
            {
                this.moveCb += moveCb;
            }
        }
        public CustomRectangleSelector(Vector2 offset, Color lineColor, float segmentSize = 5f, bool showCoordinates = false)
        {
            activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });

            if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)

                activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse, modifiers = EventModifiers.Command });

            else

                activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse, modifiers = EventModifiers.Control });

            m_Rectangle = new RectangleSelect(lineColor, segmentSize, showCoordinates);

            m_Rectangle.style.position = Position.Absolute;
            m_Rectangle.style.top = offset.y;
            m_Rectangle.style.left = offset.x;
            m_Rectangle.style.bottom = 0f;
            m_Rectangle.style.right = 0f;

            m_Active = false;
        }

        // get the axis aligned bound
        public Rect ComputeAxisAlignedBound(Rect position, Matrix4x4 transform)
        {
            Vector3 min = transform.MultiplyPoint3x4(position.min);

            Vector3 max = transform.MultiplyPoint3x4(position.max);

            return Rect.MinMaxRect(Math.Min(min.x, max.x), Math.Min(min.y, max.y), Math.Max(min.x, max.x), Math.Max(min.y, max.y));
        }


        protected override void RegisterCallbacksOnTarget()
        {
            var graphView = target as GraphView;

            if (graphView == null)
            {
                throw new InvalidOperationException("Manipulator can only be added to a GraphView");
            }

            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
            target.RegisterCallback<MouseUpEvent>(OnMouseUp);
            target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
            target.RegisterCallback<MouseCaptureOutEvent>(OnMouseCaptureOutEvent);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
            target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
            target.UnregisterCallback<MouseCaptureOutEvent>(OnMouseCaptureOutEvent);
        }

        void OnMouseCaptureOutEvent(MouseCaptureOutEvent e)
        {
            if (m_Active)
            {
                m_Rectangle.RemoveFromHierarchy();
                m_Active = false;
            }
        }

        private void OnMouseDown(MouseDownEvent e)
        {
            downCb?.Invoke(e);
            if (m_Active)
            {
                e.StopImmediatePropagation();
                return;
            }

            var graphView = target as GraphView;
            if (graphView == null)
                return;

            if (CanStartManipulation(e))
            {
                if (!e.actionKey)
                {
                    graphView.ClearSelection();
                }

                graphView.Add(m_Rectangle);

                m_Rectangle.start = e.localMousePosition;
                m_Rectangle.end = m_Rectangle.start;

                m_Active = true;
                target.CaptureMouse(); // We want to receive events even when mouse is not over ourself.
                e.StopImmediatePropagation();
            }
        }

        private void OnMouseUp(MouseUpEvent e)
        {
            upCb?.Invoke(e);
            if (!m_Active)
                return;

            var graphView = target as GraphView;
            if (graphView == null)
                return;

            if (!CanStopManipulation(e))
                return;

            graphView.Remove(m_Rectangle);

            m_Rectangle.end = e.localMousePosition;

            var selectionRect = new Rect()
            {
                min = new Vector2(Math.Min(m_Rectangle.start.x, m_Rectangle.end.x), Math.Min(m_Rectangle.start.y, m_Rectangle.end.y)),
                max = new Vector2(Math.Max(m_Rectangle.start.x, m_Rectangle.end.x), Math.Max(m_Rectangle.start.y, m_Rectangle.end.y))
            };

            selectionRect = ComputeAxisAlignedBound(selectionRect, graphView.viewTransform.matrix.inverse);

            List<ISelectable> selection = graphView.selection;

            // a copy is necessary because Add To selection might cause a SendElementToFront which will change the order.
            List<ISelectable> newSelection = new List<ISelectable>();
            graphView.graphElements.ForEach(child =>
            {
                var localSelRect = graphView.contentViewContainer.ChangeCoordinatesTo(child, selectionRect);
                if (child.IsSelectable() && child.Overlaps(localSelRect))
                {
                    newSelection.Add(child);
                }
            });

            foreach (var selectable in newSelection)
            {
                if (selection.Contains(selectable))
                {
                    if (e.actionKey) // invert selection on shift only
                        graphView.RemoveFromSelection(selectable);
                }
                else
                    graphView.AddToSelection(selectable);
            }

            m_Active = false;
            target.ReleaseMouse();
            e.StopPropagation();
        }

        private void OnMouseMove(MouseMoveEvent e)
        {
            moveCb?.Invoke(e);
            if (!m_Active)
                return;

            m_Rectangle.end = e.localMousePosition;
            e.StopPropagation();
        }

        private class RectangleSelect : ImmediateModeElement
        {
            MethodInfo applyWireMaterial = null;

            public Vector2 start { get; set; }
            public Vector2 end { get; set; }

            Color lineColor;

            float segmentSize;

            bool showCoordinates;

            string Format(string fmt, params object[] args)
            {
                return String.Format(CultureInfo.InvariantCulture.NumberFormat, fmt, args);
            }

            public RectangleSelect(Color lineColor, float segmentSize, bool showCoordinates)
            {
                this.lineColor = lineColor;

                this.segmentSize = segmentSize;

                this.showCoordinates = showCoordinates;

                var methods = typeof(HandleUtility).GetMethods(BindingFlags.NonPublic | BindingFlags.Static);

                foreach (var method in methods)
                {
                    var parameters = method.GetParameters();

                    if (method.Name == "ApplyWireMaterial" && parameters.Length == 0)
                    {
                        applyWireMaterial = method;
                    }
                }
            }

            protected override void ImmediateRepaint()
            {
                VisualElement t = parent;
                Vector2 screenStart = start;
                Vector2 screenEnd = end;

                // Avoid drawing useless information
                if (start == end)
                    return;

                // Apply offset
                screenStart += t.layout.position;
                screenEnd += t.layout.position;

                var r = new Rect
                {
                    min = new Vector2(Math.Min(screenStart.x, screenEnd.x), Math.Min(screenStart.y, screenEnd.y)),
                    max = new Vector2(Math.Max(screenStart.x, screenEnd.x), Math.Max(screenStart.y, screenEnd.y))
                };

                Vector3[] points =
                {
                    new Vector3(r.xMin, r.yMin, 0.0f),
                    new Vector3(r.xMax, r.yMin, 0.0f),
                    new Vector3(r.xMax, r.yMax, 0.0f),
                    new Vector3(r.xMin, r.yMax, 0.0f)
                };

                DrawDottedLine(points[0], points[1], segmentSize, lineColor);
                DrawDottedLine(points[1], points[2], segmentSize, lineColor);
                DrawDottedLine(points[2], points[3], segmentSize, lineColor);
                DrawDottedLine(points[3], points[0], segmentSize, lineColor);

                if (showCoordinates)
                {
                    var str = "(" + Format("{0:0}", start.x) + ", " + Format("{0:0}", start.y) + ")";

                    GUI.skin.label.Draw(new Rect(screenStart.x, screenStart.y - 18.0f, 200.0f, 20.0f), new GUIContent(str), 0);

                    str = "(" + Format("{0:0}", end.x) + ", " + Format("{0:0}", end.y) + ")";

                    GUI.skin.label.Draw(new Rect(screenEnd.x - 80.0f, screenEnd.y + 5.0f, 200.0f, 20.0f), new GUIContent(str), 0);
                }
            }

            private void DrawDottedLine(Vector3 p1, Vector3 p2, float segmentsLength, Color col)
            {
                //HandleUtility.ApplyWireMaterial();

                // Use reflection to call HandleUtility.ApplyWireMaterial(), since it's not public
                applyWireMaterial.Invoke(null, null);

                GL.Begin(GL.LINES);

                GL.Color(col);

                float length = Vector3.Distance(p1, p2); // ignore z component

                int count = Mathf.CeilToInt(length / segmentsLength);

                for (int i = 0; i < count; i += 2)
                {
                    GL.Vertex((Vector3.Lerp(p1, p2, i * segmentsLength / length)));

                    GL.Vertex((Vector3.Lerp(p1, p2, (i + 1) * segmentsLength / length)));
                }

                GL.End();
            }
        }
    }
}

#endif