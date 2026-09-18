using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        [SerializeField] private Transform heartContainer;
        [SerializeField] private GameIcons.HeartIcon heartIconPrefab;

        [Header("Mana")]
        // [SerializeField] private Image manaFillImage;
        [SerializeField] private TextMeshProUGUI manaCountText; // swap for TMP_Text in a real project


        [Header("Visibility")]
        [SerializeField] private CanvasGroup canvasGroup;

        private readonly List<GameIcons.HeartIcon> _hearts = new();

        private void Start()
        {
            // delete content under heartContainer
            foreach (Transform child in heartContainer)
            {
                Destroy(child.gameObject);
            }

            // Ensure the canvas group is hidden at start
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            // TODO: Remove this and use actual bootstrap code to bind.
            // Test: test purpose
            HUDPresenter presenter = new HUDPresenter(this);
            presenter.Bind(new HUDModel(5, 100f)); // 5 hearts, 100 mana
            presenter.Show(true); // Show instantly for testing
        }

        public void SetHealth(int current, int max)
        {
            int heartsNeeded = max;
            EnsureHeartCount(heartsNeeded);

            for (int i = 0; i < _hearts.Count; i++)
            {
                int heartValue = Mathf.Clamp(current - i, 0, 1); // 0 empty, 1 full
                _hearts[i].SetState(heartValue);
            }
        }

        public void SetMana(float current, float max)
        {
            // manaFillImage.fillAmount = max > 0f ? current / max : 0f;
            manaCountText.text = $"Mana: {Mathf.FloorToInt(current)}/{Mathf.FloorToInt(max)}";
        }
        public void Show(bool instant)
        {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
            if (instant)
            {
                canvasGroup.alpha = 1f;
            }
            else
            {
                
            }
        }

        public void Hide(bool instant)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            if (instant)
            {
                canvasGroup.alpha = 0f;
            }
            else
            {
                
            }
        }

        private void EnsureHeartCount(int needed)
        {
            while (_hearts.Count < needed)
                _hearts.Add(Instantiate(heartIconPrefab, heartContainer));

            for (int i = 0; i < _hearts.Count; i++)
                _hearts[i].gameObject.SetActive(i < needed);
        }
    }
}