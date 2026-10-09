using UnityEngine;
using UnityEngine.SceneManagement;
using GameApp.SceneManagement;

/// <summary>
/// Singleton placed in Bootstrap scene 
/// </summary>
[DefaultExecutionOrder(-1000)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private SceneLoader _sceneLoader;
    [SerializeField] private SimpleScreenController _screenController;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitializeActiveScene();
    }

    private void OnEnable()
    {
        if (_sceneLoader == null) return;

        _sceneLoader.SceneGroupLoaded += OnSceneGroupLoaded;
        _sceneLoader.SceneAdditivelyLoaded += OnSceneAdditivelyLoaded;
    }
    
    private void OnDisable()
    {
        if (_sceneLoader == null) return;

        _sceneLoader.SceneGroupLoaded -= OnSceneGroupLoaded;
        _sceneLoader.SceneAdditivelyLoaded -= OnSceneAdditivelyLoaded;
    }

    // ==============
    // void : GameManager.Instance.Pause(bool pause);
    // void : GameManager.Instance.SwitchScene(string sceneGroupName);
    // void : GameManager.Instance.AddScene(string scenePath, bool setActiveScenne = true);
    // ==============
    #region Public Function Call
    public void Pause(bool pause)
    {
        PauseManager.SetGamePause(pause);
    }

    public void SwitchScene(string sceneGroupName)
    {
        _sceneLoader.FadeIn(async () =>
        {
            Pause(true);
            GameScreenManager.Reset();
            await _sceneLoader.LoadSceneGroup(sceneGroupName);
        });
    }

    /// <summary>
    /// Add individual scene additively.
    /// </summary>
    /// <param name="setActiveScene">Make it as default scene to instantiate object.</param>
    public async void AddScene(string scenePath, bool setActiveScene = true)
    {
        await _sceneLoader.LoadSceneAdditive(scenePath, setActiveScene);
    }
    #endregion

    #region OnMultiSceneLoaded
    private void OnSceneGroupLoaded(SceneGroup sceneGroup) => InitializeActiveScene();
    private void OnSceneAdditivelyLoaded(Scene scene) => InitializeActiveScene();
    private void InitializeActiveScene()
    {
        SceneInitializer initializer = FindSceneInitializer();
        if (initializer == null)
        {
            FinishSceneSwitch(null);
            return;
        }

        Debug.Log("Initializer Fetched, Attempt initialize");
        initializer.Initialize(() =>
        {
            FinishSceneSwitch(initializer);
        });
    }
    
    /// <summary>
    /// Haven't think of the situation that has multiple scene got SceneInitializer.cs yet.
    /// </summary>
    /// <returns></returns>
    private SceneInitializer FindSceneInitializer()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        foreach (GameObject rootObject in activeScene.GetRootGameObjects())
        {
            SceneInitializer initializer = rootObject.GetComponentInChildren<SceneInitializer>(true);
            if (initializer != null)
            {
                return initializer;
            }
        }

        return null;
    }
    #endregion

    #region Start & fade out Loading Scene
    private void FinishSceneSwitch(SceneInitializer initializer)
    {
        _sceneLoader.FadeOut();
        initializer?.OnSceneStart();
        Pause(false);
    }
    #endregion
}