using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GameIcons
{
    /// <summary>
    /// A single heart driven by two radial-filled images.
    /// fill         = front image (red), reacts quickly.
    /// followUpFill = back image (background color), lags behind on damage
    ///                and leads ahead on heal.
    /// </summary>
    public class HeartIcon : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image fill;
        [SerializeField] private Image followUpFill;

        [Header("Damage")]
        [SerializeField] private float damageFillDuration = 0.15f;
        [SerializeField] private float followUpDelay = 0.4f;
        [SerializeField] private float followUpDuration = 0.35f;
        [SerializeField] private Color damageFlashColor = Color.white;
        [SerializeField] private float damagePunch = 0.25f;

        [Header("Heal")]
        [SerializeField] private float healFollowUpDuration = 0.15f;
        [SerializeField] private float healFillDelay = 0.1f;
        [SerializeField] private float healFillDuration = 0.35f;
        [SerializeField] private float healPunch = 0.15f;

        private float current = 1f;
        private Color baseFillColor;
        private Tween fillTween, followTween, punchTween, flashTween;

        private void Awake()
        {
            baseFillColor = fill.color;
            Configure(fill);
            Configure(followUpFill);
            ApplyInstant(current);
        }

        private void OnDestroy() => KillAll();

        private static void Configure(Image img)
        {
            img.type = Image.Type.Filled;
            img.fillMethod = Image.FillMethod.Radial360;
            img.fillOrigin = (int)Image.Origin360.Top;
            img.fillClockwise = false;
        }

        /// <summary>Compatibility with the old API: 0 = empty, 1 = half, 2 = full.</summary>
        public void SetState(int state, bool instant = false) => SetFill(state / 2f, instant);

        /// <summary>Set heart fill, 0..1.</summary>
        public void SetFill(float value, bool instant = false)
        {
            value = Mathf.Clamp01(value);

            if (instant)
            {
                ApplyInstant(value);
                return;
            }

            if (Mathf.Approximately(value, current)) return;

            if (value < current) PlayDamage(value);
            else PlayHeal(value);

            current = value;
        }

        private void PlayDamage(float target)
        {
            KillAll();
            ResetVisuals();

            // Front (red) drops fast.
            fillTween = fill.DOFillAmount(target, damageFillDuration)
                .SetEase(Ease.OutQuad)
                .SetUpdate(true);

            // Back layer waits, then catches up.
            followTween = followUpFill.DOFillAmount(target, followUpDuration)
                .SetDelay(followUpDelay)
                .SetEase(Ease.InOutQuad)
                .SetUpdate(true);

            // Flash + punch.
            flashTween = fill.DOColor(damageFlashColor, 0.05f)
                .SetLoops(2, LoopType.Yoyo)
                .SetUpdate(true);

            punchTween = transform.DOPunchScale(Vector3.one * -damagePunch, 0.3f, 8, 0.8f)
                .SetUpdate(true);
        }

        private void PlayHeal(float target)
        {
            KillAll();
            ResetVisuals();

            // Back layer jumps ahead first, red then fills into it.
            followTween = followUpFill.DOFillAmount(target, healFollowUpDuration)
                .SetEase(Ease.OutQuad)
                .SetUpdate(true);

            fillTween = fill.DOFillAmount(target, healFillDuration)
                .SetDelay(healFillDelay)
                .SetEase(Ease.OutBack)
                .SetUpdate(true);

            punchTween = transform.DOPunchScale(Vector3.one * healPunch, 0.35f, 6, 0.6f)
                .SetUpdate(true);
        }

        private void ApplyInstant(float value)
        {
            KillAll();
            ResetVisuals();
            fill.fillAmount = value;
            followUpFill.fillAmount = value;
            current = value;
        }

        private void ResetVisuals()
        {
            transform.localScale = Vector3.one;
            fill.color = baseFillColor;
        }

        private void KillAll()
        {
            fillTween?.Kill();
            followTween?.Kill();
            punchTween?.Kill();
            flashTween?.Kill();
        }
    }
}