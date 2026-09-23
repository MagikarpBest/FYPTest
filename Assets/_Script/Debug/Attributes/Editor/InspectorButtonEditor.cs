using UnityEditor;
using UnityEngine;
using System.Reflection;

[CustomEditor(typeof(MonoBehaviour), true)]
public class InspectorButtonEditor : Editor
{
    /// <summary>
    /// <para>
    /// Used in other custom editors scripts to draw buttons for methods with [InspectorButton] attribute when conflicted due to multiple custom editors for the same MonoBehaviour.
    /// </para>
    /// Example usage:
    /// <code>
    /// public override void OnInspectorGUI()
    /// {
    ///     DrawDefaultInspector();
    ///     InspectorButtonEditor.DrawButtons(this);
    ///         
    ///     ... (other custom inspector code)
    /// }
    /// </code>
    /// </summary>
    /// <param name="mono"></param>
    public static void DrawButtons(MonoBehaviour mono)
    {
        MethodInfo[] methods = mono.GetType()
            .GetMethods(BindingFlags.Instance |
                        BindingFlags.Public |
                        BindingFlags.NonPublic |
                        BindingFlags.DeclaredOnly);

        bool hasButton = false;
        foreach (MethodInfo method in methods)
        {
            if (method.GetCustomAttribute<InspectorButtonAttribute>() != null)
            {
                hasButton = true;
                break;
            }
        }

        if (!hasButton)
            return; // Nothing to draw

        GUILayout.Space(10);
        GUILayout.Label("Debug Controls (Some button only available in Play Mode)", EditorStyles.boldLabel);

        // Draw buttons
        foreach (MethodInfo method in methods)
        {
            InspectorButtonAttribute buttonAttribute =
                method.GetCustomAttribute<InspectorButtonAttribute>();

            if (buttonAttribute == null)
                continue;

            string buttonLabel = string.IsNullOrEmpty(buttonAttribute.ButtonLabel)
                ? ObjectNames.NicifyVariableName(method.Name)
                : buttonAttribute.ButtonLabel;

            bool shouldDisable = buttonAttribute.OnlyInPlayMode && !Application.isPlaying;

            GUI.enabled = !shouldDisable;
            if (GUILayout.Button(buttonLabel))
            {
                method.Invoke(mono, null);
            }
            GUI.enabled = true; // Reset GUI state
        }
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        DrawButtons((MonoBehaviour)target);
    }
}