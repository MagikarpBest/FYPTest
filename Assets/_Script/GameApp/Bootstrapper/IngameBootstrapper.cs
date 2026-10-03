using HUD;
using UnityEngine;

public class InGameBootstrapper : MonoBehaviour
{
    [SerializeField] private PlayerStatus _playerStatus;
    [SerializeField] private HUDView _hudView;
    [SerializeField] private PauseMenuScreenUI _pauseMenuScreen;
    private HUDPresenter _hudPresenter;
    private PlayerInputManager _playerInputManager;

    private void Start()
    {
        InitializeHUD();
        _playerInputManager = FindFirstObjectByType<PlayerInputManager>();
        BindInputEvents();
    }
    
    /// <summary>
    /// Initialize the HUD data binding with status locally.
    /// </summary>
    private void InitializeHUD()
    {
        if (_hudView == null || _playerStatus == null)
        {
            Debug.LogError($"{nameof(InGameBootstrapper)}: InitializeHUD has no reference to assign.", this);
            return;
        }

        _hudPresenter = new HUDPresenter(_hudView, _playerStatus);
        _hudPresenter.Initialize();
        _hudPresenter.Show(true);
    }

    private void HandleEscapePressed()
    {
        if (_pauseMenuScreen == null)
        {
            Debug.LogWarning($"{nameof(InGameBootstrapper)}: Pause menu screen is not assigned.");
            return;
        }

        if (!GameScreenManager.IsStackUIActive)
        {
            GameScreenManager.Push(_pauseMenuScreen);
        }
    }

    private void OnEnable()
    {
        BindInputEvents();
    }

    private void BindInputEvents()
    {
        if (_playerInputManager == null)
        {
            return;
        }

        _playerInputManager.OnEscapePressed -= HandleEscapePressed;
        _playerInputManager.OnEscapePressed += HandleEscapePressed;
    }

    private void OnDisable()
    {
        _playerInputManager.OnEscapePressed -= HandleEscapePressed;
    }

    private void OnDestroy()
    {
        _hudPresenter?.Dispose();
    }
}
