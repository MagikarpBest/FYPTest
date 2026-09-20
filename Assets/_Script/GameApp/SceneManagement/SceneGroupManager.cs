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
            List<string> loadedScenes = new List<string>();

            await UnloadScenes();

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

        // TODO: Don't unload the duplicated scenes.
        public async Task UnloadScenes()
        {
            List<string> scenes = new List<string>();
            //string activeScene = SceneManager.GetActiveScene().name;
            int sceneCount = SceneManager.sceneCount;

            for (int i = 0; i < sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (!scene.isLoaded) continue;

                if (scene.name == "Bootstrapper") continue;
                scenes.Add(scene.name);
            }

            AsyncOperationGroup operationGroup = new AsyncOperationGroup(sceneCount);

            foreach (string scene in scenes)
            {
                var operation = SceneManager.UnloadSceneAsync(scene);   
                if (operation == null) continue;

                operationGroup.Operations.Add(operation);

                OnSceneUnloaded?.Invoke(scene);
            }

            // Wait until all AsyncOperations in the group are done
            while (!operationGroup.IsDone)
            {
                await Task.Delay(100);
            }
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
