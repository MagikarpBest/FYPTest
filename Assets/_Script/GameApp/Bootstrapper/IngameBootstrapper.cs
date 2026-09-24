using HUD;
using Player;
using UnityEngine;

public class InGameBootstrapper : MonoBehaviour
{
    [Header("Player")]
    [SerializeField]
    private PlayerStatus _playerStatus;

    [Header("HUD")]
    [SerializeField]
    private HUDView _hudView;
    private HUDPresenter _hudPresenter;

    private void Start()
    {
        InitializeHUD();
    }

    /// <summary>
    /// Subscirbe to handle value changes.
    /// </summary>
    private void InitializePlayer()
    {
        if (_playerStatus == null)
        {
            Debug.LogError($"{nameof(InGameBootstrapper)}: playerStatus is not assigned.", this);
        }

        // TODO: _playerStatus bind with controller handle receive damage/mana/currency. And subscribe event.
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
