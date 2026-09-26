using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// TODO: Note - Currency potentially not involved in this section?
//          else maybe playerStatus itself composition with inventory which include currency ig.
public class PlayerStatus : MonoBehaviour, IPlayerStatus
{
    // HP

    [Header("Health")]
    [SerializeField]
    private int _maxHealth = 5;
    public int CurrentHealth { get; private set; }
    public int MaxHealth => _maxHealth;

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

    // Active skill

    [Header("Skills")]
    [SerializeField]
    private PlayerSkill _activeSkill;

    public PlayerSkill ActiveSkill => _activeSkill;

    [SerializeField]
    private List<PlayerSkill> _skillSlots = new();

    public int SkillSlotCount => _skillSlots.Count;

    public event Action<int, int> OnHealthChanged;
    public event Action<float, float> OnManaChanged;
    public event Action<int> OnCurrencyChanged;
    public event Action<PlayerSkill> OnActiveSkillChanged;
    public event Action<int, PlayerSkill> OnSkillSlotChanged;

    private void Awake()
    {
        CurrentHealth = Mathf.Max(0, _maxHealth);
        CurrentMana = Mathf.Max(0f, _maxMana);
        Currency = Mathf.Max(0, _startingCurrency);
        if (_skillSlots.Count > 0) _activeSkill = _skillSlots.First();
    }

    private void Update()
    {
        UpdateInvincibility();
    }

    // HP

    public void TakeDamage(int amount)
    {
        // TODO: check the damage type to decide whether affect by isInvincible.
        if (amount <= 0 || IsInvincible)
            return;

        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        StartInvincibility();
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

    // Active skill

    public void SetActiveSkill(PlayerSkill skill)
    {
        if (ActiveSkill == skill)
            return;

        _activeSkill = skill;
        OnActiveSkillChanged?.Invoke(ActiveSkill);
    }

    // Skill slots

    public PlayerSkill GetSkill(int slotIndex)
    {
        if (!IsValidSlot(slotIndex))
            return null;

        return _skillSlots[slotIndex];
    }

    public void SetSkill(int slotIndex, PlayerSkill skill)
    {
        if (!IsValidSlot(slotIndex))
            return;

        _skillSlots[slotIndex] = skill;
        OnSkillSlotChanged?.Invoke(slotIndex, skill);
    }

    public bool TrySwitchSkill(int direction)
    {
        if (direction == 0 || _skillSlots.Count <= 1)
            return false;

        int currentIndex = _skillSlots.IndexOf(ActiveSkill);

        if (currentIndex < 0)
            return false;

        int nextIndex = currentIndex + direction;

        if (nextIndex < 0)
        {
            nextIndex = _skillSlots.Count - 1;
        }
        else if (nextIndex >= _skillSlots.Count)
        {
            nextIndex = 0;
        }

        PlayerSkill nextSkill = _skillSlots[nextIndex];

        if (nextSkill == null)
            return false;

        SetActiveSkill(nextSkill);

        return true;
    }

    private bool IsValidSlot(int slotIndex)
    {
        return slotIndex >= 0 && slotIndex < _skillSlots.Count;
    }
}

[System.Flags]
public enum PlayerActionRestrictions
{
    None = 0,
    RestrictMovement = 1 << 0,
    RestrictJump = 1 << 1,
    RestrictRotation = 1 << 2,
    All = ~0
}