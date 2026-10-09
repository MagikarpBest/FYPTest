using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public sealed class EntityWorldHealthBar : MonoBehaviour
{
    [Header("References")]
    private IHealthSource _healthSource;
    [Tooltip("Main (foreground) bar. Min 0, Max 1.")]
    [SerializeField] private Slider _slider;
    [Tooltip("White trailing bar drawn BEHIND the main slider. Min 0, Max 1.")]
    [SerializeField] private Slider _delaySlider;
    [Tooltip("Child object that holds the visuals and gets moved on show/hide. Falls back to this transform.")]
    [SerializeField] private RectTransform _content;
    [Tooltip("Fades the whole bar in/out. Auto-fetched from this object if empty.")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [Tooltip("Shield/cover overlay (with its own CanvasGroup) shown when a hit is blocked.")]
    [SerializeField] private CanvasGroup _guardCover;

    [Header("Visibility")]
    [SerializeField] private float _visibleDuration = 5f;
    [SerializeField] private float _fadeInDuration = 0.2f;
    [SerializeField] private float _showDuration = 0.35f;
    [SerializeField] private float _showOffsetY = 90f;
    [SerializeField] private float _fadeOutDuration = 0.3f;
    [SerializeField] private float _hideOffsetY = 30f;
    [SerializeField] private bool _hideOnDestroyed = true;
    [SerializeField] private float _destroyedHideDelay = 0.6f;

    [Header("Fill")]
    [SerializeField] private float _fillDuration = 0.1f;
    [SerializeField] private float _delayBarWait = 0.1f;
    [SerializeField] private float _delayBarDuration = 0.2f;

    [Header("Guard Effect")]
    [SerializeField] private float _guardFadeInDuration = 0.05f;
    [SerializeField] private float _guardHoldDuration = 0.35f;
    [SerializeField] private float _guardFadeOutDuration = 0.2f;
    [SerializeField] private float _guardPunchStrength = 0.15f;

    private Canvas _canvas;
    private Vector2 _basePosition;

    private bool _initialized;
    private bool _isShown;
    private float _targetValue = 1f;

    private Tween _visibilityTween;
    private Tween _hideTimer;
    private Tween _fillTween;
    private Tween _delayTween;
    private Tween _guardTween;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();

        if (_canvasGroup == null)
            _canvasGroup = GetComponent<CanvasGroup>();

        if (_content == null)
            _content = (RectTransform)transform;

        if (_healthSource == null)
            _healthSource = GetComponentInParent<IHealthSource>();

        _basePosition = _content.anchoredPosition;

        ApplyHiddenState();

        if (_guardCover != null)
            _guardCover.alpha = 0f;
    }

    private void Start()
    {
        // Sync once here instead of relying on the Destructible's initial event,
        // because Start order between the two scripts isn't guaranteed.
        if (!_initialized && _healthSource != null)
            SnapTo(Normalize(_healthSource.CurrentHealth, _healthSource.MaxHealth));
    }

    private void OnEnable()
    {
        if (_healthSource == null)
            return;

        _healthSource.OnHealthChanged += HandleHealthChanged;
        _healthSource.OnDamageBlocked += HandleDamageBlocked;
    }

    private void OnDisable()
    {
        if (_healthSource == null)
            return;

        _healthSource.OnHealthChanged -= HandleHealthChanged;
        _healthSource.OnDamageBlocked -= HandleDamageBlocked;
    }

    private void OnDestroy()
    {
        DOTween.Kill(this);
    }

    // ---------------------------------------------------------------- Events

    private void HandleHealthChanged(float currentHealth, float maxHealth)
    {
        float normalized = Normalize(currentHealth, maxHealth);

        // First value we receive is the initial sync: apply it without any animation.
        if (!_initialized)
        {
            SnapTo(normalized);
            return;
        }

        if (Mathf.Approximately(normalized, _targetValue))
            return;

        bool isDamage = normalized < _targetValue;
        _targetValue = normalized;

        AnimateFill(normalized, isDamage);
        Show();

        bool destroyed = normalized <= 0f && _hideOnDestroyed;
        ScheduleHide(destroyed ? _destroyedHideDelay : _visibleDuration);
    }

    private void HandleDamageBlocked()
    {
        Show();
        ScheduleHide(_visibleDuration);
        PlayGuardEffect();
    }

    // ------------------------------------------------------------------ Fill

    private void SnapTo(float normalized)
    {
        _initialized = true;
        _targetValue = normalized;

        if (_slider != null)
            _slider.value = normalized;

        if (_delaySlider != null)
            _delaySlider.value = normalized;
    }

    private void AnimateFill(float normalized, bool isDamage)
    {
        _fillTween?.Kill();
        _delayTween?.Kill();

        // Main bar reacts almost instantly.
        if (_slider != null)
        {
            _fillTween = _slider
                .DOValue(normalized, _fillDuration)
                .SetEase(Ease.OutQuad)
                .SetId(this);
        }

        if (_delaySlider == null)
            return;

        if (isDamage)
        {
            // White bar waits a beat, then slowly catches up to the main bar.
            _delayTween = _delaySlider
                .DOValue(normalized, _delayBarDuration)
                .SetDelay(_delayBarWait)
                .SetEase(Ease.OutCubic)
                .SetId(this);
        }
        else
        {
            // Healing: the trail must never be lower than the main bar, so move together.
            _delayTween = _delaySlider
                .DOValue(normalized, _fillDuration)
                .SetEase(Ease.OutQuad)
                .SetId(this);
        }
    }

    // ------------------------------------------------------------ Visibility

    private void Show()
    {
        if (_isShown)
            return;

        _isShown = true;

        if (_canvas != null)
            _canvas.enabled = true;

        _visibilityTween?.Kill();

        // Starting from fully hidden: begin below the resting position so it "jumps" up.
        if (_canvasGroup != null && _canvasGroup.alpha < 0.01f)
            _content.anchoredPosition = _basePosition + Vector2.down * _showOffsetY;

        Sequence seq = DOTween.Sequence().SetId(this);

        if (_canvasGroup != null)
            seq.Join(_canvasGroup.DOFade(1f, _fadeInDuration));

        seq.Join(_content.DOAnchorPosY(_basePosition.y, _showDuration).SetEase(Ease.OutBack));

        _visibilityTween = seq;
    }

    private void Hide()
    {
        if (!_isShown)
            return;

        _isShown = false;
        _visibilityTween?.Kill();

        Sequence seq = DOTween.Sequence().SetId(this);

        if (_canvasGroup != null)
            seq.Join(_canvasGroup.DOFade(0f, _fadeOutDuration));

        seq.Join(_content
            .DOAnchorPosY(_basePosition.y - _hideOffsetY, _fadeOutDuration)
            .SetEase(Ease.InQuad));

        seq.OnComplete(() =>
        {
            if (_canvas != null)
                _canvas.enabled = false;
        });

        _visibilityTween = seq;
    }

    private void ScheduleHide(float delay)
    {
        _hideTimer?.Kill();
        _hideTimer = DOVirtual.DelayedCall(delay, Hide).SetId(this);
    }

    private void ApplyHiddenState()
    {
        _isShown = false;

        if (_canvasGroup != null)
            _canvasGroup.alpha = 0f;

        _content.anchoredPosition = _basePosition + Vector2.down * _hideOffsetY;

        if (_canvas != null)
            _canvas.enabled = false;
    }

    // ----------------------------------------------------------------- Guard

    private void PlayGuardEffect()
    {
        if (_guardCover == null)
            return;

        _guardTween?.Kill();

        _guardCover.alpha = 0f;
        _guardCover.transform.localScale = Vector3.one;

        Sequence seq = DOTween.Sequence().SetId(this);
        seq.Append(_guardCover.DOFade(1f, _guardFadeInDuration));
        seq.Join(_guardCover.transform.DOPunchScale(Vector3.one * _guardPunchStrength, 0.25f, 8, 0.8f));
        seq.AppendInterval(_guardHoldDuration);
        seq.Append(_guardCover.DOFade(0f, _guardFadeOutDuration));
        seq.OnKill(() => _guardCover.transform.localScale = Vector3.one);

        _guardTween = seq;
    }

    // ---------------------------------------------------------------- Helpers

    private static float Normalize(float current, float max)
    {
        return max > 0f ? Mathf.Clamp01(current / max) : 0f;
    }
}