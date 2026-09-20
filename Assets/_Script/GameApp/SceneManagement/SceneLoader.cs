using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GameApp.SceneManagement
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private SceneGroup[] sceneGroups;

        private float _targetProgress = 0f;
        private bool _isLoading = false;
        [SerializeField] private Canvas _loadingScreen;

        public readonly SceneGroupManager sceneGroupManager = new SceneGroupManager();

        void Awake()
        {
            sceneGroupManager.OnSceneLoaded += sceneName => Debug.Log("Scene loaded: " + sceneName);
        }

        async private void Start()
        {
            await LoadSceneGroup(0);
        }

        // private void Update()
        // {
        //     if (!_isLoading) return;


        //     float currentFillAmount = loadingScreen.LoadingBar.fillAmount;
        //     float progressDif = Mathf.Abs(currentFillAmount - _targetProgress);

        //     float dynamicFillSpeed = progressDif * loadingScreen.FillSpeed;
        //     loadingScreen.LoadingBar.fillAmount = Mathf.Lerp(currentFillAmount, _targetProgress, Time.deltaTime * dynamicFillSpeed);
        // }

        public async Task LoadSceneGroup(int index)
        {
            // _loadingScreen.LoadingBar.fillAmount = 0f;
            _targetProgress = 1f;

            if (index < 0 || index >= sceneGroups.Length)
            {
                Debug.LogError(this + " Invalid scene group index " + index);
                return;
            }

            LoadingProgress loadingProgress = new LoadingProgress();
            loadingProgress.OnProgress += target => _targetProgress = Mathf.Max(target, _targetProgress);

            EnableLoadingCanvas(true);
            await sceneGroupManager.LoadScenes(sceneGroups[index], loadingProgress);
            EnableLoadingCanvas(false);
        }

        public async Task LoadSceneGroup(string name)
        {
            // _loadingScreen.LoadingBar.fillAmount = 0f;
            _targetProgress = 1f;

            var group = Array.Find(sceneGroups, g => g.Name == name);

            if (group == null)
            {
                Debug.LogError(this + " Invalid scene group name: " + name);
                return;
            }

            LoadingProgress loadingProgress = new LoadingProgress();
            loadingProgress.OnProgress += target => _targetProgress = Mathf.Max(target, _targetProgress);

            _isLoading = true;

            EnableLoadingCanvas(true);
            await sceneGroupManager.LoadScenes(group, loadingProgress);
            EnableLoadingCanvas(false);
        }

        void EnableLoadingCanvas(bool enable = true)
        {
            _isLoading = enable;
            _loadingScreen.gameObject.SetActive(enable);
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
