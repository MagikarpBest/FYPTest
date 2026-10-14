using System;

namespace HUD
{
    /// <summary>
    /// Contract the Presenter talks to. Presenter never touches GameObjects/Animator/Image directly
    /// </summary>
    public interface IHUDView
    {

        void SetHealth(float current, float max);
        void SetMana(float current, float max);
        void SetAbility(int slotIndex, AbilityDisplayDataSO skill);
        void SetActiveAbility(AbilityDisplayDataSO skill);

        void Show(bool instant);
        void Hide(bool instant);
    }
}