using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using Eflatun.SceneReference; // Additional Package used to check scene is inside build profile/active or not.

namespace GameApp.SceneManagement
{
    [CreateAssetMenu(fileName = "SceneGroup", menuName = "Scriptable Objects/SceneGroup")]
    public class SceneGroup : ScriptableObject
    {
        public string Name;
        public List<SceneReference> Scenes = new List<SceneReference>();

    #if UNITY_EDITOR
        // For editor use only, to open the scene group in the editor.
        public void Open()
        {
            for (int i = 0; i < Scenes.Count; i++)
            {
                EditorSceneManager.OpenScene(Scenes[i].Path, i == 0 ? OpenSceneMode.Single : OpenSceneMode.Additive);
            }

            SetActiveScene(GetActiveScenePath());
        }
    #endif

        public string GetActiveScenePath()
        {
            foreach (SceneReference scene in Scenes)
            {
                if (scene != null && !IsBootstrapScene(scene.Path))
                {
                    return scene.Path;
                }
            }

            return string.Empty;
        }

        private static bool IsBootstrapScene(string scenePath)
        {
            string sceneName = Path.GetFileNameWithoutExtension(scenePath);
            return sceneName == "AppBootstrap" || sceneName == "IngameBootstrap";
        }

        private static void SetActiveScene(string scenePath)
        {
            if (string.IsNullOrEmpty(scenePath)) return;

            Scene scene = SceneManager.GetSceneByPath(scenePath);
            if (scene.IsValid() && scene.isLoaded)
            {
                SceneManager.SetActiveScene(scene);
            }
        }
    }
}