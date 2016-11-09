/*using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomPropertyDrawer (typeof (QualityList))]
public class NestedStringList : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PrefixLabel(position, label);
        EditorGUI.PropertyField(position, property.FindPropertyRelative("Qualities"));
    }
}*/
