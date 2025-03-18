using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

using UnityFramework.UI;

[CustomEditor(typeof(UIBase), true)]
public class UIBaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space(10.0f);

        if (GUILayout.Button("ToggleCanvas"))
        {
            UIBase ui = (UIBase)target;
            ui?.EditorActiveToggle();
        }

        EditorGUILayout.Space(5.0f);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Show All Children"))
        {
            UIBase ui = (UIBase)target;
            if (ui != null)
            {
                var bases = ui.transform.GetComponentsInChildren<UIBase>();
                foreach(var _base in bases)
                    _base.EditorShow(); 
            }
        }

        if (GUILayout.Button("Hide All Children"))
        {
            UIBase ui = (UIBase)target;
            if (ui != null)
            {
                var bases = ui.transform.GetComponentsInChildren<UIBase>();
                foreach (var _base in bases)
                    _base.EditorHide();
            }
        }
        EditorGUILayout.EndHorizontal();

    }
}
