using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameApp.SceneManagement
{
    public class SceneGroupManager 
    {
        public event Action<string> OnSceneLoaded;
        public event Action<string> OnSceneUnloaded;
        public event Action OnSceneGroupLoaded;

        // The bootstrap scene is always loaded, so we don't need to keep track of it
        private SceneGroup _activeSceneGroup;

        public async Task LoadScenes(SceneGroup sceneGroup, IProgress<float> progress, bool reloadDupScenes = false)
        {
            if (sceneGroup == null)
            {
                Debug.LogError("Cannot load a null scene group");
                return;
            }

            _activeSceneGroup = sceneGroup;
            HashSet<string> scenePathsToKeep = new HashSet<string>(
                sceneGroup.Scenes
                    .Where(scene => scene != null && !string.IsNullOrEmpty(scene.Path))
                    .Select(scene => scene.Path),
                StringComparer.OrdinalIgnoreCase);

            await UnloadScenes(scenePathsToKeep);

            List<string> loadedScenes = new List<string>();

            int sceneCount = SceneManager.sceneCount;
            for (int i = 0; i < sceneCount; i++)
            {
                loadedScenes.Add(SceneManager.GetSceneAt(i).path);
            }

            // Load the scenes in the scene group
            int totalScenesToLoad = _activeSceneGroup.Scenes.Count;
            AsyncOperationGroup operationGroup = new AsyncOperationGroup(totalScenesToLoad);
            for (int i = 0; i < totalScenesToLoad; i++)
            {
                string scenePath = sceneGroup.Scenes[i].Path;
                if (string.IsNullOrEmpty(scenePath)) continue;
                if (reloadDupScenes == false && loadedScenes.Contains(scenePath)) continue; // Prevent duplicated scene loading

                var operation = SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Additive);
                if (operation == null)
                {
                    Debug.LogError($"Could not load scene '{scenePath}' from scene group '{sceneGroup.Name}'");
                    continue;
                }

                operationGroup.Operations.Add(operation);

                OnSceneLoaded?.Invoke(scenePath);
            }

            // Wait until all AsyncOperations in the group are done.
            while (!operationGroup.IsDone)
            {
                progress?.Report(operationGroup.Progress);
                await Task.Delay(100);
            }

            string activeScenePath = sceneGroup.GetActiveScenePath();
            if (!string.IsNullOrEmpty(activeScenePath))
            {
                Scene activeScene = SceneManager.GetSceneByPath(activeScenePath);
                if (activeScene.IsValid() && activeScene.isLoaded)
                {
                    SceneManager.SetActiveScene(activeScene);
                }
                else
                {
                    Debug.LogError($"Could not set active scene '{activeScenePath}' for scene group '{sceneGroup.Name}'");
                }
            }

            OnSceneGroupLoaded?.Invoke();
        }

        public async Task<bool> LoadSceneAdditive(string scenePath, IProgress<float> progress = null, bool setActiveScene = false)
        {
            if (string.IsNullOrEmpty(scenePath))
            {
                Debug.LogError("Cannot load an additive scene without a scene path");
                return false;
            }

            Scene scene = SceneManager.GetSceneByPath(scenePath);
            if (scene.IsValid() && scene.isLoaded)
            {
                if (setActiveScene)
                {
                    SceneManager.SetActiveScene(scene);
                }

                return true;
            }

            AsyncOperation operation = SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Additive);
            if (operation == null)
            {
                Debug.LogError($"Could not load additive scene '{scenePath}'");
                return false;
            }

            while (!operation.isDone)
            {
                progress?.Report(operation.progress);
                await Task.Delay(100);
            }

            scene = SceneManager.GetSceneByPath(scenePath);
            if (setActiveScene && scene.IsValid() && scene.isLoaded)
            {
                SceneManager.SetActiveScene(scene);
            }

            OnSceneLoaded?.Invoke(scenePath);
            return true;
        }

        public async Task UnloadScenes(IEnumerable<string> scenePathsToKeep = null)
        {
            HashSet<string> pathsToKeep = new HashSet<string>(
                scenePathsToKeep ?? Enumerable.Empty<string>(),
                StringComparer.OrdinalIgnoreCase);
            List<Scene> scenesToUnload = new List<Scene>();
            int sceneCount = SceneManager.sceneCount;

            for (int i = 0; i < sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (!scene.isLoaded) continue;

                if (IsPersistentScene(scene) || pathsToKeep.Contains(scene.path)) continue;
                scenesToUnload.Add(scene);
            }

            AsyncOperationGroup operationGroup = new AsyncOperationGroup(sceneCount);

            foreach (Scene scene in scenesToUnload)
            {
                var operation = SceneManager.UnloadSceneAsync(scene);
                if (operation == null) continue;

                operationGroup.Operations.Add(operation);

                OnSceneUnloaded?.Invoke(scene.path);
            }

            // Wait until all AsyncOperations in the group are done
            while (!operationGroup.IsDone)
            {
                await Task.Delay(100);
            }
        }

        private static bool IsPersistentScene(Scene scene)
        {
            return scene.name == "AppBootstrap";
        }
    }

    public readonly struct AsyncOperationGroup
    {
        public readonly List<AsyncOperation> Operations;

        public float Progress => Operations.Count == 0 ? 0f : Operations.Average(o => o.progress);
        public bool IsDone => Operations.All(o => o.isDone);

        public AsyncOperationGroup(int initialCapacity)
        {
            Operations = new List<AsyncOperation>(initialCapacity);
        }
    }
}
