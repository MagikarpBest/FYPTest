using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(StatModifier))]
public class StatModifierPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var statType = property.FindPropertyRelative("StatType");
        var magnitudeType = property.FindPropertyRelative("MagnitudeType");
        var operation = property.FindPropertyRelative("Operation");
        var floatMagnitude = property.FindPropertyRelative("FloatMagnitude");
        var tagMagnitude = property.FindPropertyRelative("TagMagnitude");

        // position.height = EditorGUIUtility.singleLineHeight;

        EditorGUI.PropertyField(position, statType);
        //position.y += EditorGUIUtility.singleLineHeight + 2;

        EditorGUI.PropertyField(position, magnitudeType);
        //position.y += EditorGUIUtility.singleLineHeight + 2;

        EditorGUI.PropertyField(position, operation);
        //position.y += EditorGUIUtility.singleLineHeight + 2;

        if (magnitudeType.enumValueIndex == (int)StatModifier.ModifierMagnitudeType.Float)
        {
            EditorGUI.PropertyField(position, floatMagnitude);
        }
        else
        {
            EditorGUI.PropertyField(position, tagMagnitude);
        }

        EditorGUI.EndProperty();
    }

    // public override float GetPropertyHeight(
    //     SerializedProperty property,
    //     GUIContent label)
    // {
    //     return (EditorGUIUtility.singleLineHeight + 2) * 4;
    // }
}
