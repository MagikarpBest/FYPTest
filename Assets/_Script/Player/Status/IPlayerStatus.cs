using System;

namespace Player
{
    public interface IPlayerStatus
    {
        // HP
        int CurrentHealth { get; }
        int MaxHealth { get; }

        // Mana
        float CurrentMana { get; }
        float MaxMana { get; }

        // Currency
        int Currency { get; }

        // Active skill
        PlayerSkill ActiveSkill { get; }

        // Skill slots
        int SkillSlotCount { get; }
        PlayerSkill GetSkill(int slotIndex);

        event Action<int, int> OnHealthChanged;
        event Action<float, float> OnManaChanged;
        event Action<int> OnCurrencyChanged;
        event Action<PlayerSkill> OnActiveSkillChanged;
        event Action<int, PlayerSkill> OnSkillSlotChanged;

        void TakeDamage(int amount);
        void Heal(int amount);

        void ConsumeMana(float amount);
        void AddMana(float amount);

        void AddCurrency(int amount);
        bool TrySpendCurrency(int amount);

        void SetActiveSkill(PlayerSkill skill);
        void SetSkill(int slotIndex, PlayerSkill skill);
        bool TrySwitchSkill(int direction);
    }
}