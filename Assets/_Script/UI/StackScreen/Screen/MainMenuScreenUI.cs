using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Main menu screen: Play/Options/Credit/Quit with a moving selection indicator.
/// Auto-selects Play when shown. Left-click on empty space won't clear the selection.
/// </summary>
public class MainMenuScreenUI : ScreenBase
{
    [Header("Buttons")]
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _optionsButton;
    [SerializeField] private Button _creditButton;
    [SerializeField] private Button _quitButton;

    [Header("Navigate To (optional)")]
    [Tooltip("Pushed via GameScreenManager when clicked. Leave empty if not wired up yet.")]
    [SerializeField] private ScreenBase _optionsScreen;
    [SerializeField] private ScreenBase _creditScreen;
    [SerializeField] private string _playSceneName = "EexuanScene";

    [Header("Selection Indicator")]
    [Tooltip("The Image/Object that visually highlights the currently selected button.")]
    [SerializeField] private RectTransform _selectionIndicator;
    [SerializeField] private bool _matchButtonSize = true;
    [SerializeField] private float _moveDuration = 0.15f;
    [SerializeField] private Ease _moveEase = Ease.OutQuad;

    private Button[] _buttons;
    private GameObject _lastSelected;

    private Tween _moveTween;
    private Tween _pulseTween;

    // Separate from ScreenBase.Awake() (which is private, not virtual).
    // Unity still calls Awake once per class level in the hierarchy, so both run independently.
    private void Start()
    {
        _buttons = new[] { _playButton, _optionsButton, _creditButton, _quitButton };

        _playButton.onClick.AddListener(OnPlayClicked);
        _optionsButton.onClick.AddListener(OnOptionsClicked);
        _creditButton.onClick.AddListener(OnCreditClicked);
        _quitButton.onClick.AddListener(OnQuitClicked);

        foreach (Button b in _buttons)
        {
            AddHoverSelect(b);
        }
    }

    public override bool ShouldHonorBackButton()
    {
        return false;
    }

    private void AddHoverSelect(Button button)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        entry.callback.AddListener(_ => Select(button));
        trigger.triggers.Add(entry);
    }

    protected override void OnShow()
    {
        Select(_playButton);
    }

    public override void Focus()
    {
        base.Focus();

        // Returning from a sub-screen with nothing selected (e.g. Options closed) -> restore.
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            Select(_lastSelected != null ? _lastSelected.GetComponent<Button>() : _playButton);
        }
    }

    private void Update()
    {
        if (CanvasGroup.interactable == false) return;
        GameObject current = EventSystem.current.currentSelectedGameObject;

        if (current == null)
        {
            // Clicking empty space deselects by default - pull it back instead of losing it.
            Select(_lastSelected != null ? _lastSelected.GetComponent<Button>() : _playButton);
            return;
        }

        if (current != _lastSelected && IsOwnButton(current))
        {
            _lastSelected = current;
            MoveIndicatorTo(current.GetComponent<RectTransform>(), instant: false);
        }
    }

    private bool IsOwnButton(GameObject obj)
    {
        foreach (Button b in _buttons)
        {
            if (b != null && b.gameObject == obj) return true;
        }
        return false;
    }

    private void Select(Button button)
    {
        if (button == null) return;

        EventSystem.current.SetSelectedGameObject(button.gameObject);
        _lastSelected = button.gameObject;
        MoveIndicatorTo(button.GetComponent<RectTransform>(), instant: false);
    }

    private void MoveIndicatorTo(RectTransform target, bool instant)
    {
        if (_selectionIndicator == null || target == null) return;

        if (_matchButtonSize)
        {
            _selectionIndicator.sizeDelta = target.sizeDelta;
        }

        _moveTween?.Kill();

        if (instant)
        {
            _selectionIndicator.position = target.position;
        }
        else
        {
            _moveTween = _selectionIndicator.DOMove(target.position, _moveDuration).SetEase(_moveEase);
        }
    }

    private void OnPlayClicked()
    {
        // TODO: hook up real scene switch, e.g. GameManager.Instance.SwitchScene("Gameplay");
        GameManager.Instance.SwitchScene(_playSceneName);
    }

    private void OnOptionsClicked()
    {
        if (_optionsScreen != null)
            GameScreenManager.Push(_optionsScreen);
        else
            Debug.Log("Options clicked (no screen wired up yet)");
    }

    private void OnCreditClicked()
    {
        if (_creditScreen != null)
            GameScreenManager.Push(_creditScreen);
        else
            Debug.Log("Credit clicked (no screen wired up yet)");
    }

    private void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}