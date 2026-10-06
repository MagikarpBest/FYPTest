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
            return new MainToolbarButton(new MainToolbarContent("Scenes", assetIcon, "Opens the scene selection tool."), 
                OpenSearchableMenu);
        }

        private static void OnOpenScenesMenu()
        {
            GenericMenu menu = new GenericMenu();

            // string[] guids = AssetDatabase.FindAssets("t:SceneAsset");
            // foreach (string guid in guids)
            // {
            //     string path = AssetDatabase.GUIDToAssetPath(guid);
            //     string sceneName = System.IO.Path.GetFileNameWithoutExtension(path);
            //     menu.AddItem(new GUIContent(sceneName), false, () => OpenScene(path));
            // }

            string[] guids = AssetDatabase.FindAssets("t:SceneGroup");

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                SceneGroup sceneGroup = AssetDatabase.LoadAssetAtPath<SceneGroup>(path);

                if (sceneGroup == null)
                    continue;

                menu.AddItem(new GUIContent(sceneGroup.Name), false, () => 
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        sceneGroup.Open();
                    }
                });
            }

            menu.ShowAsContext();
        }

        private static void OpenSearchableMenu()
        {
            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), new SceneSearchableMenu());
        }
        
        
        private static void OpenScene(string path)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(path);
            }
        }
    }
}