using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;

// Source: https://www.youtube.com/watch?v=XFIkkDDFHfs
namespace GameApp.SceneManagement.Editor
{
    public class EditorToolbarSceneButton
    {
        [MainToolbarElement("Scene Button", defaultDockPosition = MainToolbarDockPosition.Right)]
        public static MainToolbarButton CreateButton()
        {
            Texture2D assetIcon = EditorGUIUtility.IconContent("SceneAsset Icon").image as Texture2D;
            return new MainToolbarButton(new MainToolbarContent("Scenes", assetIcon, "Opens the scene selection tool."), OpenSearchableMenu);
        }

        private static void OpenSearchableMenu()
        {
            Vector2 mousePosition = Event.current != null
                ? Event.current.mousePosition
                : new Vector2(100f, 100f);

            Vector2 screenPosition = GUIUtility.GUIToScreenPoint(mousePosition);
            SceneSearchableMenu menu = ScriptableObject.CreateInstance<SceneSearchableMenu>();
            SearchWindow.Open(new SearchWindowContext(screenPosition),menu);
        }
    }
}