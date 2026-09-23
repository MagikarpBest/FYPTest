// --------------------------------------------------------------
// Creation Date: 2026-02-21 11:46
// Author: nyuig
// Description: This code contents created by ChatGPT, used for creating debug button in inspector
// Usage:
//      1. Create a method in any MonoBehaviour script
//      2. Add [InspectorButton] attribute to the method, and optionally provide a label for the button.
//
//      Example 1:
//          [InspectorButton("Test Button")]
//          private void TestMethod()
//          {
//              Debug.Log("Test button clicked!");
//          }
//      Example 2:
//          [InspectorButton("Test Button", true)] // Only show button in play mode
//          private void TestMethod()
//          {
//              Debug.Log("Test button clicked!");
//          }
// --------------------------------------------------------------
using System;


/// <summary>
/// This custom attribute is used to make a button appear in the inspector for a method. 
/// When the button is clicked, the method for test purpose will be invoked in the editor.
/// <para> Currently only work MonoBehaviour, ScriptableObject don't have this demand ig.</para>
/// <code> 
/// // Default: label = null; onlyInPlayMode = false
///     [InspectorButton("This is button label", true)] or 
///     [InspectorButton] // (will use method name as label)
///     private void TestMethod()
/// </code>
/// </summary>
public class InspectorButtonAttribute : Attribute
{
    public string ButtonLabel;
    public bool OnlyInPlayMode;

    public InspectorButtonAttribute(string label = null, bool onlyInPlayMode = false)
    {
        ButtonLabel = label;
        OnlyInPlayMode = onlyInPlayMode;
    }
}
