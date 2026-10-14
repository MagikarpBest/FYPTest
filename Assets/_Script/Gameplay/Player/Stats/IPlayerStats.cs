using System;

public interface IPlayerStats
{
    // HP
    float CurrentHealth { get; }
    float MaxHealth { get; }

    // Mana
    float CurrentMana { get; }
    float MaxMana { get; }

    // Currency
    int Currency { get; }

    
    event Action<float, float> OnHealthChanged;
    event Action<float, float> OnManaChanged;
    event Action<int> OnCurrencyChanged;


    void Heal(int amount);

    void ConsumeMana(float amount);
    void AddMana(float amount);

    void AddCurrency(int amount);
    bool TrySpendCurrency(int amount);
    
}

