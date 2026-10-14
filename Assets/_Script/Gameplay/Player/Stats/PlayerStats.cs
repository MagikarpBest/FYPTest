using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// TODO: Note - Currency potentially not involved in this section?
//          else maybe playerStatus itself composition with inventory which include currency ig.
public class PlayerStats : MonoBehaviour, IDamageable
{
    // HP

    [Header("Health")]
    [SerializeField]
    private float _maxHealth = 5;
    public float CurrentHealth { get; private set; }
    public float MaxHealth => _maxHealth;

    [SerializeField]
    private float _invincibilityDuration = 1f;
    public bool IsInvincible { get; private set; }
    private float _invincibilityTimer;

    // Mana
    [Header("Mana")]
    [SerializeField]
    private float _maxMana = 5.0f;
    public float CurrentMana { get; private set; }
    public float MaxMana => _maxMana;

    // Currency
    [Header("Currency")]
    [SerializeField]
    private int _startingCurrency = 0;

    public int Currency { get; private set; }
    
    public event Action<float, float> OnHealthChanged;
    public event Action<float, float> OnManaChanged;
    public event Action<int> OnCurrencyChanged;

    private PlayerRewardCollector _rewardCollector;

    private void Awake()
    {
        CurrentHealth = Mathf.Max(0, _maxHealth);
        CurrentMana = Mathf.Max(0f, _maxMana);
        Currency = Mathf.Max(0, _startingCurrency);

        _rewardCollector = new PlayerRewardCollector(this); // instant create and subscribe to death events(static) to any entities.
    }

    private void Update()
    {
        UpdateInvincibility();
    }

    // HP

    public DamageResult TakeDamage(DamageData damageData)
    {
        // TODO: check the damage type to decide whether affect by isInvincible.
        if (damageData.Damage <= 0 || IsInvincible)
            return DamageResult.Blocked;

        CurrentHealth = Mathf.Max(0, CurrentHealth - damageData.Damage);
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        StartInvincibility();
        return DamageResult.Damaged;
    }

    public void Heal(int amount)
    {
        if (amount <= 0)
            return;

        CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + amount);
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }

    private void StartInvincibility()
    {
        IsInvincible = true;
        _invincibilityTimer = _invincibilityDuration;
    }

    private void UpdateInvincibility()
    {
        if (!IsInvincible)
            return;

        _invincibilityTimer -= Time.deltaTime; // TODO: Handle pause

        if (_invincibilityTimer <= 0f)
        {
            IsInvincible = false;
            _invincibilityTimer = 0f;
        }
    }

    // Mana
    public bool CheckMana(float amount)
    {
        if (CurrentMana - amount >= 0) return true;
        return false;
    }
    public void ConsumeMana(float amount)
    {
        if (amount <= 0f)
            return;

        CurrentMana = Mathf.Max(0f, CurrentMana - amount);
        OnManaChanged?.Invoke(CurrentMana, MaxMana);
    }

    public void AddMana(float amount)
    {
        if (amount <= 0f)
            return;

        CurrentMana = Mathf.Min(MaxMana, CurrentMana + amount);
        OnManaChanged?.Invoke(CurrentMana, MaxMana);
    }

    // Currency

    public void AddCurrency(int amount)
    {
        if (amount <= 0)
            return;

        Currency += amount;
        OnCurrencyChanged?.Invoke(Currency);
    }

    public bool TrySpendCurrency(int amount)
    {
        if (amount <= 0 || Currency < amount)
            return false;

        Currency -= amount;
        OnCurrencyChanged?.Invoke(Currency);

        return true;
    }

   
}

