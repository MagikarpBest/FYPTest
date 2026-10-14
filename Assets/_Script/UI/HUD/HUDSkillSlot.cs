using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HUD
{
    public class HUDSkillSlot : MonoBehaviour
    {
        [SerializeField]
        private Image _icon;

        [SerializeField]
        private GameObject _selectedIndicator;

        [SerializeField]
        private TMP_Text _skillNameText;
        public AbilityDisplayDataSO Ability { get; private set; }

        public void SetAbility(AbilityDisplayDataSO ability)
        {
            Ability = ability;

            bool hasSkill = ability != null;

            _icon.enabled = hasSkill;
            _skillNameText.gameObject.SetActive(hasSkill);

            if (!hasSkill)
            {
                _icon.sprite = null;
                _skillNameText.text = string.Empty;
                return;
            }

            if (ability == null)
            {
                return;
            }
            _icon.sprite = ability.Icon;
            _skillNameText.text = ability.AbilityName;
        }

        public void SetSelected(bool selected)
        {
            _selectedIndicator.SetActive(selected);
        }
    }
}
