#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TreeDesigner.Runtime;

namespace TreeDesigner.Editor
{
    [CustomPropertyDrawer(typeof(ExposedProperty), true)]
    public class ExposedPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.objectReferenceValue is ExposedProperty exposedProperty && exposedProperty.Global)
            {
                GUI.enabled = false;
                EditorGUI.ObjectField(position, property, label);
                GUI.enabled = true;
            }
            else
            {
                List<UnityEngine.Object> properties = new List<UnityEngine.Object>();
                List<string> propertyNames = new List<string>();
                List<GUIContent> guiContents = new List<GUIContent>();

                properties.Add(null);
                propertyNames.Add("Empty");
                guiContents.Add(new GUIContent("Empty"));

                foreach (var item in ExposedProperty.exposedProperties)
                {
                    var target = property.objectReferenceValue;

                    var a = property.propertyType;
                    var b = property.type;


                    var c = item.GetType().Name;

                    bool contains = b.Contains(c);
                    if (contains)
                    {
                        propertyNames.Add(item.Name);
                        properties.Add(item);
                        guiContents.Add(new GUIContent(item.Name));
                    }
                }

                var valueRect = new Rect(position.x, position.y, position.width - 20, position.height);

                EditorGUI.BeginChangeCheck();
                var index = EditorGUI.Popup(valueRect, label, Math.Max(0, properties.IndexOf(property.objectReferenceValue)), guiContents.ToArray());
                if (EditorGUI.EndChangeCheck())
                {
                    property.objectReferenceValue = properties[index];
                }
            }          
        }
    }
}

#endif