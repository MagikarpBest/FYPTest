using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Provide Serialize data binding with actual script that implement inspector.
/// Pros: 
///     Ignore the factory patterns from analyze the content with a potentially duplicable ID.
/// </summary>
[CustomPropertyDrawer(typeof(IPlayerSkillHandler), true)]
public class PlayerSkillHandlerDrawer : PropertyDrawer
{
    private static readonly List<Type> DerivedTypes;

    static PlayerSkillHandlerDrawer()
    {
        DerivedTypes = TypeCache
            .GetTypesDerivedFrom<IPlayerSkillHandler>()
            .Where(type =>
                !type.IsAbstract &&
                !type.IsInterface &&
                !type.IsGenericType)
            .ToList();
    }

    public override void OnGUI(
        Rect position,
        SerializedProperty property,
        GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        float yOffset = 0f;

        // Dropdown
        Rect dropdownRect = new Rect(
            position.x,
            position.y + yOffset,
            position.width,
            EditorGUIUtility.singleLineHeight);

        string currentTypeName = property.managedReferenceFullTypename;
        int currentIndex = -1;

        for (int i = 0; i < DerivedTypes.Count; i++)
        {
            if (!string.IsNullOrEmpty(currentTypeName) &&
                currentTypeName.Contains(DerivedTypes[i].FullName))
            {
                currentIndex = i;
                break;
            }
        }

        string[] options = DerivedTypes
            .Select(type => type.Name)
            .ToArray();

        int newIndex = EditorGUI.Popup(
            dropdownRect,
            label.text,
            currentIndex,
            options);

        if (newIndex != currentIndex && newIndex >= 0)
        {
            property.managedReferenceValue =
                Activator.CreateInstance(DerivedTypes[newIndex]);

            property.serializedObject.ApplyModifiedProperties();
        }

        yOffset += EditorGUIUtility.singleLineHeight + 2;

        // Draw child fields
        if (property.managedReferenceValue != null)
        {
            EditorGUI.indentLevel++;

            SerializedProperty iterator = property.Copy();
            SerializedProperty end = iterator.GetEndProperty();

            iterator.NextVisible(true);

            while (!SerializedProperty.EqualContents(iterator, end))
            {
                float height = EditorGUI.GetPropertyHeight(
                    iterator,
                    true);

                Rect childRect = new Rect(
                    position.x,
                    position.y + yOffset,
                    position.width,
                    height);

                EditorGUI.PropertyField(
                    childRect,
                    iterator,
                    true);

                yOffset += height + 2;
                iterator.NextVisible(false);
            }

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(
        SerializedProperty property,
        GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight;

        if (property.managedReferenceValue == null)
            return height;

        SerializedProperty iterator = property.Copy();
        SerializedProperty end = iterator.GetEndProperty();

        iterator.NextVisible(true);

        while (!SerializedProperty.EqualContents(iterator, end))
        {
            height += EditorGUI.GetPropertyHeight(
                iterator,
                true) + 2;

            iterator.NextVisible(false);
        }

        return height;
    }
}