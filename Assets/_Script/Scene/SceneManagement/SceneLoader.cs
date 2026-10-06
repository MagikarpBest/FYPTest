using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

namespace GameApp.SceneManagement
{
    public class SceneLoader : MonoBehaviour
    {
        public event Action<SceneGroup> SceneGroupLoaded;
        public event Action<Scene> SceneAdditivelyLoaded;

        [SerializeField] private SceneGroup[] sceneGroups;

        private float _targetProgress = 0f;
        private bool _isLoading = false;
        [SerializeField] private Canvas _loadingScreen;
        [SerializeField] private CanvasGroup _loadingCanvasGroup;
        [SerializeField] private float _loadTransitionDuration = 0.25f;
        [SerializeField] private float _resumeTransitionDuration = 0.25f;

        public readonly SceneGroupManager sceneGroupManager = new SceneGroupManager();

        public bool IsLoading => _isLoading;

        private void Awake()
        {
            sceneGroupManager.OnSceneLoaded += sceneName => Debug.Log("Scene loaded: " + sceneName);
        }

        private async Task<bool> LoadSceneGroupInternal(SceneGroup group)
        {
            if (group == null) return false;

            _targetProgress = 1f;

            var loadingProgress = new LoadingProgress();
            loadingProgress.OnProgress += target => _targetProgress = Mathf.Max(target, _targetProgress);

            _isLoading = true;
            await sceneGroupManager.LoadScenes(group, loadingProgress);
            _isLoading = false;

            SceneGroupLoaded?.Invoke(group);
            return true;
        }

        public Task<bool> LoadSceneGroup(int index)
        {
            if (index < 0 || index >= sceneGroups.Length)
            {
                Debug.LogError(this + " Invalid scene group index " + index);
                return Task.FromResult(false);
            }
            return LoadSceneGroupInternal(sceneGroups[index]);
        }

        public Task<bool> LoadSceneGroup(string name)
        {
            var group = Array.Find(sceneGroups, g => g.Name == name);
            if (group == null)
            {
                Debug.LogError(this + " Invalid scene group name: " + name);
                return Task.FromResult(false);
            }
            return LoadSceneGroupInternal(group);
        }

        public async Task<bool> LoadSceneAdditive(string scenePath, bool setActiveScene = false)
        {
            LoadingProgress loadingProgress = new LoadingProgress();
            loadingProgress.OnProgress += target => _targetProgress = Mathf.Max(target, _targetProgress);

            _isLoading = true;
            bool loaded = await sceneGroupManager.LoadSceneAdditive(scenePath, loadingProgress, setActiveScene);
            _isLoading = false;

            if (loaded)
            {
                SceneAdditivelyLoaded?.Invoke(SceneManager.GetSceneByPath(scenePath));
            }

            return loaded;
        }

        private IEnumerator Fade(bool fadeIn, Action callback = null)
        {
            Debug.Log("Fade in > " + fadeIn); 
            float duration = fadeIn ? _loadTransitionDuration : _resumeTransitionDuration;
            if (fadeIn) _loadingScreen.gameObject.SetActive(true);

            float start = fadeIn ? 0f : 1f;
            float end = fadeIn ? 1f : 0f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                _loadingCanvasGroup.alpha = Mathf.Lerp(start, end, elapsed / duration);
                yield return null;
            }

            _loadingCanvasGroup.alpha = end;
            if (!fadeIn) _loadingScreen.gameObject.SetActive(false);
            callback?.Invoke();
        }

        public void FadeIn(Action callback = null) => StartCoroutine(Fade(true, callback));
        public void FadeOut(Action callback = null) => StartCoroutine(Fade(false, callback));

        [InspectorButton("Load First Scene Group", true)]
        private void TestLoadSceneGroup()
        {
            GameManager.Instance.SwitchScene(sceneGroups[0].Name);
        }
    }

    public class LoadingProgress : IProgress<float>
    {
        public event Action<float> OnProgress;

        const float ratio = 1f;

        public void Report(float value)
        {
            OnProgress?.Invoke(value / ratio);
        }
    }
}
