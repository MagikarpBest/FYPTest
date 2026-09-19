using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameApp.SceneManagement.Editor
{
    public class SceneSearchableMenu : ScriptableObject, ISearchWindowProvider
    {
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> tree = new List<SearchTreeEntry>();

            SearchTreeGroupEntry group = new SearchTreeGroupEntry(new GUIContent("Scenes Assets"), 0);
            tree.Add(group);

            SearchTreeGroupEntry collection = new SearchTreeGroupEntry(new GUIContent("Collections"), 1);
            tree.Add(collection);

            SceneGroup[] sceneGroups = Resources.FindObjectsOfTypeAll<SceneGroup>();
            foreach (SceneGroup sceneGroup in sceneGroups)
            {
                SearchTreeEntry entry = new SearchTreeEntry(new GUIContent(sceneGroup.Name))
                {
                    level = 2,
                    userData = sceneGroup
                };
                tree.Add(entry);
            }

            

            SearchTreeGroupEntry scenes = new SearchTreeGroupEntry(new GUIContent("Scenes"), 1);
            tree.Add(scenes);

            string[] guids = AssetDatabase.FindAssets("t:SceneAsset");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                SearchTreeEntry entry = new SearchTreeEntry(new GUIContent(sceneAsset.name))
                {
                    level = 2,
                    userData = sceneAsset
                };
                tree.Add(entry);
            }


            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry entry, SearchWindowContext context)
        {
            if (entry.userData is SceneGroup sceneGroup)
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    sceneGroup.Open();
                }
                return true;
            }

            if (entry.userData is SceneAsset sceneAsset)
            {
                if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    return false;
                }
                OpenSceneWithBootstrap(sceneAsset);
                return true;
            }
            return false;
        }

        private static void OpenSceneWithBootstrap(SceneAsset sceneAsset)
        {
            string scenePath = AssetDatabase.GetAssetPath(sceneAsset);

            SceneAsset appBootstrap = FindSceneAsset("AppBootstrap");
            SceneAsset ingameBootstrap = FindSceneAsset("IngameBootstrap");

            if (appBootstrap == null)
            {
                Debug.LogError("AppBootstrap scene could not be found.");
                return;
            }

            if (ingameBootstrap == null)
            {
                Debug.LogError("IngameBootstrap scene could not be found.");
                return;
            }

            string appBootstrapPath =
                AssetDatabase.GetAssetPath(appBootstrap);

            string ingameBootstrapPath =
                AssetDatabase.GetAssetPath(ingameBootstrap);

            // Open AppBootstrap as the base scene.
            EditorSceneManager.OpenScene(appBootstrapPath,
                OpenSceneMode.Single);

            // Add IngameBootstrap.
            EditorSceneManager.OpenScene(
                ingameBootstrapPath,
                OpenSceneMode.Additive);

            // Add the selected gameplay scene.
            EditorSceneManager.OpenScene(
                scenePath,
                OpenSceneMode.Additive);

            Scene activeScene = SceneManager.GetSceneByPath(scenePath);
            if (activeScene.IsValid())
            {
                SceneManager.SetActiveScene(activeScene);
            }
        }

        /// <summary>
        /// Helper function
        /// </summary>
        /// <param name="sceneName">Bootstrap scene name</param>
        private static SceneAsset FindSceneAsset(string sceneName)
        {
            string[] guids = AssetDatabase.FindAssets("t:SceneAsset");

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                if (Path.GetFileNameWithoutExtension(path) != sceneName)
                {
                    continue;
                }

                SceneAsset sceneAsset =
                    AssetDatabase.LoadAssetAtPath<SceneAsset>(path);

                if (sceneAsset != null)
                {
                    return sceneAsset;
                }
            }

            return null;
        }
    }
}