using HUD;
using UnityEngine;

public class InGameBootstrapper : MonoBehaviour
{
    [SerializeField] private PlayerStatus _playerStatus;
    [SerializeField] private HUDView _hudView;
    private HUDPresenter _hudPresenter;

    private void Start()
    {
        InitializeHUD();
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

    private void OnDestroy()
    {
        _hudPresenter?.Dispose();
    }
}
