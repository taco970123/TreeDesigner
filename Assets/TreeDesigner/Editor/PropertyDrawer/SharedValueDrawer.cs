#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TreeDesigner.Runtime;

namespace TreeDesigner.Editor
{
    [CustomPropertyDrawer(typeof(SharedValue),true)]
    public class SharedValueDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            List<UnityEngine.Object> properties = new List<UnityEngine.Object>();
            List<string> propertyNames = new List<string>();
            List<GUIContent> guiContents = new List<GUIContent>();

            properties.Add(null);
            propertyNames.Add("Empty");
            guiContents.Add(new GUIContent("Empty"));

            foreach (var item in ExposedProperty.exposedProperties)
            {
                var value = property.FindPropertyRelative("value");
                var so = new SerializedObject(item);
                var soValue = so.FindProperty("value");

                if(value.propertyType == soValue.propertyType)
                {
                    propertyNames.Add(item.Name);
                    properties.Add(item);
                    guiContents.Add(new GUIContent(item.Name));
                }
            }

            var valueRect = new Rect(position.x, position.y, position.width - 20, position.height);
            var useSharedRect = new Rect(position.width - 15, position.y, position.height, position.height);

            EditorGUI.BeginChangeCheck();
            var index = EditorGUI.Popup(valueRect, label, Math.Max(0, properties.IndexOf(property.FindPropertyRelative("exposedProperty").objectReferenceValue)), guiContents.ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                property.FindPropertyRelative("exposedProperty").objectReferenceValue = properties[index];
            }
        }
    }
}
#endif