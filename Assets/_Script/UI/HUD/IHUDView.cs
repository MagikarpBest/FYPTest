using System;

namespace HUD
{
    /// <summary>
    /// Contract the Presenter talks to. Presenter never touches GameObjects/Animator/Image directly
    /// </summary>
    public interface IHUDView
    {
        event Action<int> OnSkillSwitchInput;

        void SetHealth(float current, float max);
        void SetMana(float current, float max);
        void SetSkill(int slotIndex, PlayerSkill skill);
        void SetActiveSkill(PlayerSkill skill);
        void PlaySkillSwitchPressed();
        void PlaySkillSwitchFailed();

        void Show(bool instant);
        void Hide(bool instant);
    }
}