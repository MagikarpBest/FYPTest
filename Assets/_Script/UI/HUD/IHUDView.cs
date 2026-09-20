using System.Collections.Generic;

namespace HUD
{
    /// <summary>
    /// Contract the Presenter talks to. Presenter never touches GameObjects/Animator/Image directly
    /// </summary>
    public interface IHUDView
    {
        void SetHealth(int current, int max);
        void SetMana(float current, float max);
        // void SetSkills

        void Show(bool instant);
        void Hide(bool instant);
    }
}