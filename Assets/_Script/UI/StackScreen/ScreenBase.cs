using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(GraphicRaycaster))]
public class ScreenBase : MonoBehaviour
{
    private Canvas canvas;
    private CanvasGroup group;

    private const float FADE_DURATION = 0.2f;

    private Coroutine cachedCoroutine = null;

    // Protected properties in case child classes wants to override focus/unfocus
    // and not wanting to call base methods.
    protected Canvas Canvas => canvas;

    protected CanvasGroup CanvasGroup => group;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        group = GetComponent<CanvasGroup>();

        canvas.enabled = false;
        group.interactable = false;
    }

    // Whether this screen listens to back button
    // override and return false if unwanted
    // e.g. forced answer confirmation pop up
    public virtual bool ShouldHonorBackButton()
    {
        return true;
    }

    // To be called by SimpleScreenManager
    public void Show(bool instant)
    {
        ClearCurrentCoroutine();

        // force this screen to be on top
        transform.SetAsLastSibling();

        OnShow();

        if (instant)
        {
            canvas.enabled = true;
            group.alpha = 1.0f;
            group.interactable = true;
        }
        else
        {
            cachedCoroutine = StartCoroutine(ShowCoroutine());
        }
    }

    // To be called by SimpleScreenManager
    public void Hide(bool instant)
    {
        ClearCurrentCoroutine();
        if (instant)
        {
            canvas.enabled = false;
            group.alpha = 0.0f;
            group.interactable = false;
        }
        else
        {
            cachedCoroutine = StartCoroutine(HideCoroutine());
        }
    }

    // To be called by SimpleScreenManager
    // Focus() is called when this screen gains focus.
    // happens after screen on top of this screen being popped.
    public virtual void Focus()
    {
        group.interactable = true;
    }

    // To be called by SimpleScreenManager
    // Unfocus() is called before another screen shows on top.
    public virtual void Unfocus()
    {
        group.interactable = false;
    }

    // Called by Show()
    protected virtual void OnShow()
    { }

    // Called by Show()
    // All screens fade in in 0.2 seconds (FADE_DURATION) in linear manner
    private IEnumerator ShowCoroutine()
    {
        float start = Time.time;
        float end = start + FADE_DURATION;

        canvas.enabled = true;
        group.alpha = 0.0f;
        group.interactable = false;
        while (Time.time < end)
        {
            float t = (Time.time - start) / FADE_DURATION;

            group.alpha = t;
            yield return null;
        }

        group.alpha = 1.0f;
        group.interactable = true;
    }

    // Called by Hide()
    // All screens fade out in 0.2 seconds (FADE_DURATION) in linear manner
    private IEnumerator HideCoroutine()
    {
        float start = Time.time;
        float end = start + FADE_DURATION;

        group.alpha = 1.0f;
        group.interactable = false;
        while (Time.time < end)
        {
            float t = (Time.time - start) / FADE_DURATION;

            group.alpha = 1.0f - t;
            yield return null;
        }

        canvas.enabled = false;
        group.alpha = 0.0f;
    }

    // Called by Show() and Hide()
    private void ClearCurrentCoroutine()
    {
        if (cachedCoroutine != null)
        {
            StopCoroutine(cachedCoroutine);
        }
    }
}