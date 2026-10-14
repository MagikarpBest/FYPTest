using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

namespace HUD
{
    // TODO: Add skills list and subscribe to action input from HUDView.

    /// <summary>
    /// Owns "how" the HUD looks and animates. Never reaches into HudModel - only ever
    /// receives already-resolved values (int/float/list) from the Presenter.
    /// </summary>
    public class HUDView : MonoBehaviour, IHUDView
    {
        [Header("Health")]
        [SerializeField] private Transform _heartContainer;
        [SerializeField] private GameIcons.HeartIcon _heartIconPrefab;

        [Header("Mana")]
        [SerializeField] private TextMeshProUGUI _manaCountText; // swap for TMP_Text in a real project

        [SerializeField]
        private Transform _skillContainer;

        [SerializeField]
        private HUDSkillSlot _skillSlotPrefab;

        [Header("Visibility")]
        [SerializeField] private CanvasGroup _canvasGroup;

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
        }
        

        public void SetHealth(float current, float max)
        {
            float heartsNeeded = max;
            EnsureHeartCount(heartsNeeded);

            for (int i = 0; i < _hearts.Count; i++)
            {
                float heartValue = Mathf.Clamp(current - i, 0, 1); // 0 empty, 1 full
                _hearts[i].SetState(heartValue);
            }
        }

        public void SetMana(float current, float max)
        {
            // manaFillImage.fillAmount = max > 0f ? current / max : 0f;
            _manaCountText.text = $"Mana: {Mathf.FloorToInt(current)}/{Mathf.FloorToInt(max)}";
        }

        public void SetAbility(int slotIndex, AbilityDisplayDataSO skill)
        {
            EnsureSkillSlotCount(slotIndex + 1);
            _skillSlots[slotIndex].SetAbility(skill);
        }

        public void SetActiveAbility(AbilityDisplayDataSO skill)
        {
            for (int i = 0; i < _skillSlots.Count; i++)
            {
                _skillSlots[i].SetSelected(_skillSlots[i].Ability == skill);
            }
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