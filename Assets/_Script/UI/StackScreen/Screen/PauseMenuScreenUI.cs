using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Pause menu screen: Resume/Options/Exit to Main Menu with a moving selection indicator.
/// Auto-selects Resume when shown. Left-click on empty space won't clear the selection.
/// Pressing the back button (Escape) resumes the game, same as clicking Resume.
/// </summary>
public class PauseMenuScreenUI : ScreenBase
{
    [Header("Buttons")]
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _optionsButton;
    [SerializeField] private Button _exitButton;

    [Header("Navigate To (optional)")]
    [Tooltip("Pushed via GameScreenManager when clicked. Leave empty if not wired up yet.")]
    [SerializeField] private ScreenBase _optionsScreen;
    [SerializeField] private string _mainMenuSceneName = "MainMenuGroup";

    [Header("Selection Indicator")]
    [Tooltip("The Image/Object that visually highlights the currently selected button.")]
    [SerializeField] private RectTransform _selectionIndicator;
    [SerializeField] private bool _matchButtonSize = true;
    [SerializeField] private float _moveDuration = 0.1f;
    [SerializeField] private Ease _moveEase = Ease.OutBack;

    private Button[] _buttons;
    private GameObject _lastSelected;

    private Tween _moveTween;
    private Tween _pulseTween;

    private void Start()
    {
        _buttons = new[] { _resumeButton, _optionsButton, _exitButton };

        _resumeButton.onClick.AddListener(OnResumeClicked);
        _optionsButton.onClick.AddListener(OnOptionsClicked);
        _exitButton.onClick.AddListener(OnExitClicked);

        foreach (Button b in _buttons)
        {
            AddHoverSelect(b);
        }
    }

    public override bool ShouldHonorBackButton()
    {
        // Cannot shut by back button, conflict with the Escape trigger pause menu.
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
        // GameManager.Instance.Pause(true);
        Select(_resumeButton);
    }

    public override void Focus()
    {
        base.Focus();

        // Returning from a sub-screen with nothing selected (e.g. Options closed) -> restore.
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            Select(_lastSelected != null ? _lastSelected.GetComponent<Button>() : _resumeButton);
        }
    }

    private void Update()
    {
        if (CanvasGroup.interactable == false) return;
        GameObject current = EventSystem.current.currentSelectedGameObject;

        if (current == null)
        {
            // Clicking empty space deselects by default - pull it back instead of losing it.
            Select(_lastSelected != null ? _lastSelected.GetComponent<Button>() : _resumeButton);
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

    private void OnResumeClicked()
    {
        // GameManager.Instance.Pause(false);
        GameScreenManager.Pop();
    }

    private void OnOptionsClicked()
    {
        if (_optionsScreen != null)
            GameScreenManager.Push(_optionsScreen);
        else
            Debug.Log("Options clicked (no screen wired up yet)");
    }

    private void OnExitClicked()
    {
        // GameManager.Instance.Pause(false);
        GameScreenManager.Pop();
        GameManager.Instance.SwitchScene(_mainMenuSceneName);
    }
}