using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public sealed class HideScriptField : Attribute
{
}

#if UNITY_EDITOR
[CustomEditor(typeof(ScriptableObject), true)]
public class DefaultMonoBehaviourEditor : Editor
{
    private bool hideScriptField;

    private void OnEnable()
    {
        hideScriptField = target.GetType().GetCustomAttributes(typeof(HideScriptField), true).Length > 0;
    }

    public override void OnInspectorGUI()
    {
        if (hideScriptField)
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            DrawPropertiesExcluding(serializedObject, "m_Script");
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }
        else
        {
            base.OnInspectorGUI();
        }
    }
}
#endif
