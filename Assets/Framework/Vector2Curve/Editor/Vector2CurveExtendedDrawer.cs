using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

using UnityFramework;

[CustomPropertyDrawer(typeof(Vector2CurveExtended), true)]
public class Vector2CurveExtendedDrawer : Vector2CurveDrawer
{
    private const string CURVE_PLAYE_MODE = "curvePlayeMode";

    protected override void ChildPropertys(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty curvePlayeMode = property.FindPropertyRelative(CURVE_PLAYE_MODE);
        EditorGUILayout.PropertyField(curvePlayeMode);
    }
}
