using UnityEditor;
using UnityEngine;
using System.Linq;

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
        }
        EditorGUI.BeginProperty(position, label, property);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label);
    }
}
