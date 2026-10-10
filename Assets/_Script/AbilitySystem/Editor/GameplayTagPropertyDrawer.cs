using System;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

[CustomPropertyDrawer(typeof(GameplayTag))]
public class GameplayTagPropertyDrawer : PropertyDrawer
{
    bool firstTimeOpen = true;
    private static string[] tagNames = new string[0];
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (firstTimeOpen)
        {
            firstTimeOpen = false;
            var tagDatabase = Resources.Load<GameplayTagDatabaseSO>("GameplayTagDatabase");
            tagNames = tagDatabase != null ? tagDatabase.Tags.ToArray() : new string[0];
            Array.Sort(tagNames);
        }
        
        EditorGUI.BeginProperty(position, label, property);
        
        Rect fieldRect = EditorGUI.PrefixLabel(position, label);

        // "name" = the string field inside your GameplayTag; change it to match
        SerializedProperty nameProp = property.FindPropertyRelative("name");
        string shown = string.IsNullOrEmpty(nameProp.stringValue) ? "None" : nameProp.stringValue;

        if (EditorGUI.DropdownButton(fieldRect, new GUIContent(shown), FocusType.Keyboard))
        {
            SerializedObject serializedObject = property.serializedObject;
            string propertyPath = property.propertyPath;

            var dropdown = new GameplayTagDropdown(new AdvancedDropdownState(), tagNames, picked =>
            {
                serializedObject.Update();
                serializedObject.FindProperty(propertyPath).FindPropertyRelative("name").stringValue = picked;
                serializedObject.ApplyModifiedProperties();
            });
            dropdown.Show(fieldRect);
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
}
