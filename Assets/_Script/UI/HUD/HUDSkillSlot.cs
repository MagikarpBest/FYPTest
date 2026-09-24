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
        public PlayerSkill Skill { get; private set; }

        public void SetSkill(PlayerSkill skill)
        {
            Skill = skill;

            bool hasSkill = skill != null;

            _icon.enabled = hasSkill;
            _skillNameText.gameObject.SetActive(hasSkill);

            if (!hasSkill)
            {
                _icon.sprite = null;
                _skillNameText.text = string.Empty;
                return;
            }

            _icon.sprite = skill.Icon;
            _skillNameText.text = skill.SkillName;
        }

        public void SetSelected(bool selected)
        {
            _selectedIndicator.SetActive(selected);
        }
    }
}