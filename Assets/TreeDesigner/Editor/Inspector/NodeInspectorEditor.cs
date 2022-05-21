#if UNITY_EDITOR
using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TreeDesigner.Runtime;

namespace TreeDesigner.Editor
{
    [CustomEditor(typeof(BaseNode), true)]
    public class NodeInspectorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SerializedProperty serializedProperty = serializedObject.GetIterator();
            serializedProperty.NextVisible(true);
            while (serializedProperty.NextVisible(false))
            {
                EditorGUI.BeginChangeCheck();
                string labelName = LabelAs(serializedProperty);
                bool showProperty = true;

                if (DisablePortProperty(serializedProperty, ref labelName) || ReadOnly(serializedProperty))
                    GUI.enabled = false;

                GUIContent label = new GUIContent(labelName);
                bool tooltip = HoverInfo(serializedProperty, label);
                if (ShowIf(serializedProperty) && !HideIf(serializedProperty) && showProperty)
                {
                    if (tooltip)
                        GUI.Label(new Rect(), GUI.tooltip);

                    EditorGUILayout.PropertyField(serializedProperty, label);
                }

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    OnValueChanged(serializedProperty);
                }

                GUI.enabled = true;
            }
        }

        protected virtual bool ShowIf(SerializedProperty serializedProperty)
        {
            ShowIf[] allIfs = GetPropertyAttributes<ShowIf>(serializedProperty);
            if (allIfs == null)
                return true;

            List<ShowIf> differentIfs = new List<ShowIf>();
            foreach (var showIf in allIfs)
            {
                if (differentIfs.Find((i) => i.Name == showIf.Name) != null)
                    continue;
                differentIfs.Add(showIf);            
            }
            foreach (var showIf in differentIfs)
            {
                bool show = false;
                Type type = GetValueType(serializedObject, showIf.Name);
                if (type == typeof(bool))
                {
                    if (showIf.Conditions != null)
                    {
                        foreach (var item in showIf.Conditions)
                        {
                            if (item is bool && item.Equals(GetBoolValue(serializedObject, showIf.Name)))
                                show = true;
                        }
                    }
                }
                else if (type.BaseType == typeof(Enum))
                {
                    if (showIf.Conditions != null)
                    {
                        foreach (var item in showIf.Conditions)
                        {
                            if (item is Enum && item.Equals(GetEnumValue(serializedObject, showIf.Name)))
                                show = true;
                        }
                    }
                }
                if (!show)
                    return false;
            }
            return true;
        }
        protected virtual bool HideIf(SerializedProperty serializedProperty)
        {
            HideIf[] allIfs = GetPropertyAttributes<HideIf>(serializedProperty);
            if (allIfs == null)
                return false;

            List<HideIf> differentIfs = new List<HideIf>();
            foreach (var showIf in allIfs)
            {
                if (differentIfs.Find((i) => i.Name == showIf.Name) != null)
                    continue;
                differentIfs.Add(showIf);
            }
            foreach (var showIf in differentIfs)
            {
                bool show = false;
                Type type = GetValueType(serializedObject, showIf.Name);
                if (type == typeof(bool))
                {
                    if (showIf.Conditions != null)
                    {
                        foreach (var item in showIf.Conditions)
                        {
                            if (item is bool && item.Equals(GetBoolValue(serializedObject, showIf.Name)))
                                show = true;
                        }
                    }
                }
                else if (type.BaseType == typeof(Enum))
                {
                    if (showIf.Conditions != null)
                    {
                        foreach (var item in showIf.Conditions)
                        {
                            if (item is Enum && item.Equals(GetEnumValue(serializedObject, showIf.Name)))
                                show = true;
                        }
                    }
                }
                if (!show)
                    return false;
            }


            return true;
        }
        protected virtual string LabelAs(SerializedProperty serializedProperty)
        {
            LabelAs[] labelAss = GetPropertyAttributes<LabelAs>(serializedProperty);
            if (labelAss == null)
                return serializedProperty.displayName;

            string label = GetStringValue(serializedObject, labelAss[0].Label);
            return label == string.Empty ? labelAss[0].Label : label;
        }
        protected virtual bool HoverInfo(SerializedProperty serializedProperty,GUIContent label)
        {
            HoverInfo[] hoverInfos = GetPropertyAttributes<HoverInfo>(serializedProperty);
            if (hoverInfos == null)
                return false;

            string tooltip = GetStringValue(serializedObject, hoverInfos[0].Tooltip);
            label.tooltip = tooltip == string.Empty ? hoverInfos[0].Tooltip : tooltip;
            return true;
        }

        protected virtual void OnValueChanged(SerializedProperty serializedProperty)
        {
            OnValueChanged[] onValueChangeds = GetPropertyAttributes<OnValueChanged>(serializedProperty);
            if (onValueChangeds == null)
                return;
            InvokeMethod(serializedObject, onValueChangeds[0].Name);
        }
        protected virtual bool ReadOnly(SerializedProperty serializedProperty)
        {
            ReadOnly[] readOnlys = GetPropertyAttributes<ReadOnly>(serializedProperty);
            if (readOnlys == null)
                return false;
            return true;
        }
        protected virtual bool DisablePortProperty(SerializedProperty serializedProperty, ref string label)
        {
            PortInfo[] portInfos = GetPropertyAttributes<PortInfo>(serializedProperty);
            if (portInfos != null)
            {
                label = portInfos[0].PortName;
            }
            BaseNode buffBaseNode = serializedObject.targetObject as BaseNode;
            foreach (var item in buffBaseNode.LinkDatas)
            {
                if (item.inputValueName == serializedProperty.name)
                    return true;
            }
            return false;
        }

        protected virtual T GetPropertyAttribute<T>(SerializedProperty serializedProperty) where T : Attribute
        {
            Type t = serializedProperty.serializedObject.targetObject.GetType();
            FieldInfo f = null;
            PropertyInfo p = null;
            foreach (var name in serializedProperty.propertyPath.Split('.'))
            {
                f = t.GetField(name);

                if (f == null)
                {
                    p = t.GetProperty(name);
                    if (p == null)
                    {
                        return default(T);
                    }
                    t = p.PropertyType;
                }
                else
                {
                    t = f.FieldType;
                }
            }

            T[] attributes = new T[0];

            if (f != null)
            {
                attributes = f.GetCustomAttributes(typeof(T), false) as T[];
            }
            else if (p != null)
            {
                attributes = p.GetCustomAttributes(typeof(T), false) as T[];
            }
            if (attributes.Length > 0)
                return attributes[0];
            return default(T);
        }
        protected virtual T[] GetPropertyAttributes<T>(SerializedProperty serializedProperty) where T : Attribute
        {
            Type t = serializedProperty.serializedObject.targetObject.GetType();
            FieldInfo f = null;
            PropertyInfo p = null;
            foreach (var name in serializedProperty.propertyPath.Split('.'))
            {
                f = t.GetField(name);

                if (f == null)
                {
                    p = t.GetProperty(name);
                    if (p == null)
                    {
                        return null;
                    }
                    t = p.PropertyType;
                }
                else
                {
                    t = f.FieldType;
                }
            }

            T[] attributes = new T[0];

            if (f != null)
            {
                attributes = f.GetCustomAttributes(typeof(T), false) as T[];
            }
            else if (p != null)
            {
                attributes = p.GetCustomAttributes(typeof(T), false) as T[];
            }
            if (attributes.Length > 0)
                return attributes;
            return null;
        }

        protected virtual Type GetValueType(SerializedObject serializedObject, string name)
        {
            Type t = serializedObject.targetObject.GetType();
            FieldInfo f = null;
            PropertyInfo p = null;
            MethodInfo m = null;

            f = t.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (f != null)
                return f.FieldType;
            p = t.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (p != null)
                return p.PropertyType;
            m = t.GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (m != null)
                return m.ReturnType;
            return null;
        }
        protected virtual bool GetBoolValue(SerializedObject serializedObject, string name)
        {
            Type t = serializedObject.targetObject.GetType();
            FieldInfo f = null;
            PropertyInfo p = null;
            MethodInfo m = null;

            f = t.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (f != null)
                return (bool)f.GetValue(serializedObject.targetObject);
            p = t.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (p != null)
                return (bool)p.GetValue(serializedObject.targetObject);
            m = t.GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (m != null)
                return (bool)m.Invoke(serializedObject.targetObject, null);
            return false;
        }
        protected virtual string GetStringValue(SerializedObject serializedObject, string name)
        {
            Type t = serializedObject.targetObject.GetType();
            FieldInfo f = null;
            PropertyInfo p = null;
            MethodInfo m = null;

            f = t.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (f != null)
                return (string)f.GetValue(serializedObject.targetObject);
            p = t.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (p != null)
                return (string)p.GetValue(serializedObject.targetObject);
            m = t.GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (m != null)
                return (string)m.Invoke(serializedObject.targetObject, null);
            return string.Empty;
        }
        protected virtual Enum GetEnumValue(SerializedObject serializedObject, string name)
        {
            Type t = serializedObject.targetObject.GetType();
            FieldInfo f = null;
            PropertyInfo p = null;
            MethodInfo m = null;

            f = t.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (f != null)
                return (Enum)f.GetValue(serializedObject.targetObject);
            p = t.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (p != null)
                return (Enum)p.GetValue(serializedObject.targetObject);
            m = t.GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (m != null)
                return (Enum)m.Invoke(serializedObject.targetObject, null);
            return null;
        }
        protected virtual void InvokeMethod(SerializedObject serializedObject, string name)
        {
            Type t = serializedObject.targetObject.GetType();
            MethodInfo m = null;

            m = t.GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (m != null)
                m.Invoke(serializedObject.targetObject, null);
        }
    }
}
#endif