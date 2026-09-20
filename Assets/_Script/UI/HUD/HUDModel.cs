using System;
using UnityEngine;

namespace HUD
{
    // TODO: Replace it with [actual data reference & event action invoked] for HUD
    // TODO: Add skills list that being actively switch and used, it can be separate file I guess.
    public class HUDModel
    {
        public int CurrentHealth { get; private set; }
        public int MaxHealth { get; private set; }
        public event Action<int, int> OnHealthChanged; // (current, max)

        public float CurrentMana { get; private set; }
        public float MaxMana { get; private set; }
        public event Action<float, float> OnManaChanged;

        public HUDModel(int maxHealth, float maxMana)
        {
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
            MaxMana = maxMana;
        }

        public void TakeDamage(int amount)
        {
            CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        }

        public void Heal(int amount)
        {
            CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + amount);
            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        }

        public void AddMana(float amount)
        {
            CurrentMana = Mathf.Clamp(CurrentMana + amount, 0f, MaxMana);
            OnManaChanged?.Invoke(CurrentMana, MaxMana);
        }
    }
}