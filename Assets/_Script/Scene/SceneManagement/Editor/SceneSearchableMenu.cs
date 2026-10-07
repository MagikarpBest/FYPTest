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
            // ----
            tree.Add(new SearchTreeGroupEntry(new GUIContent("Scenes Assets"), 0));

            tree.Add(new SearchTreeGroupEntry(new GUIContent("Collections"), 1));
            AddSceneGroups(tree);
            
            tree.Add(new SearchTreeGroupEntry(new GUIContent("Scenes"), 1));
            AddScenes(tree);

            tree.Add(new SearchTreeGroupEntry(new GUIContent("Test"), 1));
            AddTestScenes(tree);
            // ----
            return tree;
        }

        private static void AddSceneGroups(List<SearchTreeEntry> tree)
        {
            const string sceneGroupsFolder = "Assets/_Scenes/Resources";
            string[] guids = AssetDatabase.FindAssets("t:SceneGroup", new[] { sceneGroupsFolder });

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                SceneGroup sceneGroup = AssetDatabase.LoadAssetAtPath<SceneGroup>(path);

                if (sceneGroup == null) continue;

                SearchTreeEntry entry = new SearchTreeEntry(new GUIContent(sceneGroup.Name))
                {
                    level = 2,
                    userData = sceneGroup
                };

                tree.Add(entry);
            }
        }

        private static void AddScenes(List<SearchTreeEntry> tree)
        {
            const string assetsFolder = "Assets/_Scenes";
            string[] guids = AssetDatabase.FindAssets("t:SceneAsset", new[] { assetsFolder });

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                if (string.IsNullOrEmpty(path)) continue;
                if (!path.StartsWith("Assets/_Scenes/")) continue; // Extra protection against packages or external locations.
                if (path.StartsWith("Assets/_Scenes/Test")) continue;
                if (path.StartsWith("Assets/_Scenes/Bootstrap")) continue;

                SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                if (sceneAsset == null) continue;

                SearchTreeEntry entry = new SearchTreeEntry(new GUIContent(sceneAsset.name))
                {
                    level = 2,
                    userData = sceneAsset
                };

                tree.Add(entry);
            }
        }

        private static void AddTestScenes(List<SearchTreeEntry> tree)
        {
            const string assetsFolder = "Assets/_Scenes/Test";
            string[] guids = AssetDatabase.FindAssets("t:SceneAsset", new[] { assetsFolder });

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                if (string.IsNullOrEmpty(path)) continue;
                if (!path.StartsWith("Assets/_Scenes/Test/")) continue; // Extra protection against packages or external locations.

                SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                if (sceneAsset == null) continue;

                SearchTreeEntry entry = new SearchTreeEntry(new GUIContent(sceneAsset.name))
                {
                    level = 2,
                    userData = sceneAsset
                };

                tree.Add(entry);
            }
        }

        public bool OnSelectEntry(SearchTreeEntry entry, SearchWindowContext context)
        {
            if (entry.userData is SceneGroup sceneGroup)
            {
                return SelectSceneGroup(sceneGroup);
            }

            if (entry.userData is SceneAsset sceneAsset)
            {
                if (EditorApplication.isPlaying)
                {
                    Debug.LogWarning($"Cannot open individual scene '{sceneAsset.name}' " +
                        "directly during Play Mode. Select a SceneGroup instead.");
                    return false;
                }

                if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return false;

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

            string appBootstrapPath = AssetDatabase.GetAssetPath(appBootstrap);
            string ingameBootstrapPath = AssetDatabase.GetAssetPath(ingameBootstrap);

            EditorSceneManager.OpenScene(appBootstrapPath, OpenSceneMode.Single);
            EditorSceneManager.OpenScene(ingameBootstrapPath, OpenSceneMode.Additive);
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);

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

        private static bool SelectSceneGroup(SceneGroup sceneGroup)
        {
            if (EditorApplication.isPlaying)
            {
                // GameManager should be present whenever the game is running.
                GameManager gameManager = GameManager.Instance;
                if (gameManager == null)
                {
                    Debug.LogError("GameManager could not be found while the game is running.");
                    return false;
                }

                gameManager.SwitchScene(sceneGroup.Name);
                return true;
            }

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return false;

            sceneGroup.Open();
            return true;
        }
    }
}