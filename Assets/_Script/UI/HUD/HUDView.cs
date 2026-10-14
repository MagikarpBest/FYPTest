using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

namespace HUD
{
    // TODO: Add skills list and subscribe to action input from HUDView.

    /// <summary>
    /// Owns "how" the HUD looks and animates. Never reaches into HudModel - only ever
    /// receives already-resolved values (int/float/list) from the Presenter.
    /// </summary>
    public class HUDView : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] private Transform _heartContainer;
        [SerializeField] private GameIcons.HeartIcon _heartIconPrefab;

        [Header("Mana")]
        [SerializeField] private TextMeshProUGUI _manaCountText; // swap for TMP_Text in a real project

        [Header("Skills")]
        [SerializeField] private Transform _skillContainer;
        [SerializeField] private HUDSkillSlot _skillSlotPrefab;
        [SerializeField] private float _reorderDuration = 0.25f;
        [SerializeField] private Ease _reorderEase = Ease.OutCubic;

        private RectTransform _skillContainerRect;
        private int _activeIndex = -1;

        [Header("Visibility")]
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private CanvasGroup _skillSelectCanvasGroup;
        [SerializeField] private CanvasGroup _skillActiveCanvasGroup;
        private bool _isSkillActivationActive = true;

        private readonly List<GameIcons.HeartIcon> _hearts = new();
        private readonly List<HUDSkillSlot> _skillSlots = new();

        private void Awake()
        {
            // delete content under heartContainer
            foreach (Transform child in _heartContainer)
            {
                Destroy(child.gameObject);
            }

            foreach (Transform child in _skillContainer)
            {
                Destroy(child.gameObject);
            }

            // Ensure the canvas group is hidden at start
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;

            _skillContainerRect = (RectTransform)_skillContainer;
        }


        public void SetHealth(float current, float max, bool instant = false)
        {
            int heartsNeeded = Mathf.CeilToInt(max);
            int previousCount = _hearts.Count;
            EnsureHeartCount(heartsNeeded);

            for (int i = 0; i < _hearts.Count; i++)
            {
                // Hearts created this call have no previous value to animate from.
                bool isNew = i >= previousCount;
                float heartValue = Mathf.Clamp01(current - i);

                _hearts[i].SetFill(heartValue, instant || isNew);
            }
        }

        public void SetMana(float current, float max)
        {
            // manaFillImage.fillAmount = max > 0f ? current / max : 0f;
            // _manaCountText.text = $"Mana: {Mathf.FloorToInt(current)}/{Mathf.FloorToInt(max)}";
        }

        public void SetAbility(int slotIndex, AbilityDisplayDataSO skill)
        {
            EnsureSkillSlotCount(slotIndex + 1);
            _skillSlots[slotIndex].SetAbility(skill);


            // Keep order valid while the list is still being filled
            if (_activeIndex >= 0)
                ArrangeSkillSlots(animate: false);

        }

        public void SetActiveAbility(AbilityDisplayDataSO skill)
        {
            int newActive = _skillSlots.FindIndex(s => s.Ability == skill);
            if (newActive < 0) return;

            bool animate = _activeIndex >= 0 && newActive != _activeIndex;
            _activeIndex = newActive;

            for (int i = 0; i < _skillSlots.Count; i++)
            {
                // _skillSlots[i].SetSelected(_skillSlots[i].Ability == skill);
            }

            ArrangeSkillSlots(animate);
        }

        private void ArrangeSkillSlots(bool animate)
        {
            int count = _skillSlots.Count;
            if (count == 0 || _activeIndex < 0) return;

            // 1. Record current positions (also handles interrupting a running tween)
            var oldPositions = new Vector2[count];
            for (int i = 0; i < count; i++)
            {
                var rt = (RectTransform)_skillSlots[i].transform;
                rt.DOKill();
                oldPositions[i] = rt.anchoredPosition;
            }

            // 2. Reorder siblings: active skill ends up last (bottom)
            for (int pos = 0; pos < count; pos++)
            {
                int slotIndex = (_activeIndex + 1 + pos) % count;
                _skillSlots[slotIndex].transform.SetSiblingIndex(pos);
            }

            // 3. Let the Grid Layout Group compute the new target positions now
            LayoutRebuilder.ForceRebuildLayoutImmediate(_skillContainerRect);

            if (!animate) return;

            // 4. Snap back to the old position, then tween to the new one
            for (int i = 0; i < count; i++)
            {
                var rt = (RectTransform)_skillSlots[i].transform;
                Vector2 target = rt.anchoredPosition;
                if ((target - oldPositions[i]).sqrMagnitude < 0.01f) continue;

                rt.anchoredPosition = oldPositions[i];
                rt.DOAnchorPos(target, _reorderDuration)
                .SetEase(_reorderEase)
                .SetLink(rt.gameObject); // auto-kill if destroyed
            }
        }

        public void SetSkillActivationState(bool isActive)
        {
            _isSkillActivationActive = isActive;
            SetCanvasGroupVisible(_skillSelectCanvasGroup, !isActive);
            SetCanvasGroupVisible(_skillActiveCanvasGroup, isActive);
        }

        private void SetCanvasGroupVisible(CanvasGroup canvasGroup, bool isVisible)
        {
            if (canvasGroup == null)
                return;

            canvasGroup.alpha = isVisible ? 1f : 0f;
            canvasGroup.interactable = isVisible;
            canvasGroup.blocksRaycasts = isVisible;
        }

        public void PlaySkillSwitchPressed()
        {
            // TODO: Add button press visual.
        }

        public void PlaySkillSwitchFailed()
        {
            // TODO: Add failed switch shake visual.
        }

        public void Show(bool instant)
        {
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;
            if (instant)
            {
                _canvasGroup.alpha = 1f;
            }
            else
            {
                
            }
        }

        public void Hide(bool instant)
        {
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            if (instant)
            {
                _canvasGroup.alpha = 0f;
            }
            else
            {
                // TODO: Add slide in animation.
            }
        }

        private void EnsureHeartCount(float needed)
        {
            while (_hearts.Count < needed)
                _hearts.Add(Instantiate(_heartIconPrefab, _heartContainer));

            for (int i = 0; i < _hearts.Count; i++)
                _hearts[i].gameObject.SetActive(i < needed);
        }

        private void EnsureSkillSlotCount(int needed)
        {
            while (_skillSlots.Count < needed)
            {
                HUDSkillSlot slot = Instantiate(_skillSlotPrefab, _skillContainer);
                _skillSlots.Add(slot);
            }
        }
    }
}