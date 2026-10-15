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
        
        float line = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing;

        position.height = line;
        EditorGUI.PropertyField(position, statType);

        position.y += line + spacing;
        EditorGUI.PropertyField(position, operation);
        
        position.y += line + spacing;
        EditorGUI.PropertyField(position, magnitudeType);
        
        position.y += line + spacing;
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

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float line = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing;
        float elementSpacing = 8f;
        return (line * 4) + (spacing * 3) + elementSpacing;
    }
}
