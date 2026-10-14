using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HUD
{
    public class HUDSkillSlot : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        public PlayerSkill Skill { get; private set; }

        public void SetSkill(PlayerSkill skill)
        {
            Skill = skill;
            bool hasSkill = skill != null;
            _icon.enabled = hasSkill;

            if (!hasSkill)
            {
                _icon.sprite = null;
                return;
            }

            _icon.sprite = skill.Icon;
        }
    }
}