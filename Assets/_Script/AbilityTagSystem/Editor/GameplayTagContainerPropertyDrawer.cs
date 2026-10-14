using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(GameplayTagContainer))]
public class GameplayTagContainerPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(position, property.FindPropertyRelative("tags"), label, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("tags"), label, true);
    }
}

[CustomPropertyDrawer(typeof(GameplayTagCountContainer))]
public class GameplayTagCountContainerPropertyDrawer : GameplayTagContainerPropertyDrawer
{
    
}