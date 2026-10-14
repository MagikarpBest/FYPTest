using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillConfirmResult
{
    Finish = 1,
    Proceed = 2, // If skill need to confirm more than 1 steps.
    Fail = -1
}

[RequireComponent(typeof(PlayerStats))]
public class PlayerSkillController : MonoBehaviour
{
    private AbilitySystem _abilitySystem;
    private PlayerInputManager _inputManager;
    private PlayerStats _playerStats;

    [Header("Abilities")]
    [SerializeField] private AbilityDisplayDataSO _activeAbility;
    [SerializeField] private List<AbilityDisplayDataSO> _abilitySlots = new();

    // Interface implementations (matching what the HUDPresenter expects)
    public AbilityDisplayDataSO ActiveDisplayData => _activeAbility;

    public AbilityDisplayDataSO ActiveAbility => _activeAbility != null
        ? _activeAbility
        : null;

    public AbilityDisplayDataSO GetSkill(int index) => (index >= 0 && index < _abilitySlots.Count)
        ? _abilitySlots[index]
        : null;

    public int AbilitySlotCount => _abilitySlots.Count;
    public event Action<AbilityDisplayDataSO> OnActiveAbilityChanged;
    public event Action<int, AbilityDisplayDataSO> OnAbilitySlotChanged;

    private void Awake()
    {
        _abilitySystem = GetComponent<AbilitySystem>();
        _playerStats = GetComponent<PlayerStats>();
        _inputManager = FindFirstObjectByType<PlayerInputManager>();

        if (_abilitySlots.Count > 0 && _activeAbility == null)
        {
            _activeAbility = _abilitySlots[0];
        }
    }

    private void Start()
    {
        // add ability from inspector to AbilitySystem
        foreach (var displayData in _abilitySlots)
        {
            _abilitySystem.GiveAbility(displayData.Ability);
        }
    }

    private void OnEnable()
    {
        if (_inputManager == null)
        {
            Debug.LogWarning($"{nameof(PlayerSkillController)}: " + $"{nameof(PlayerInputManager)} not found.", this);
            return;
        }

        _inputManager.OnSkill3Pressed += HandleActivatePressed;

        _inputManager.OnSkillSwitchLeftPressed += () => TrySwitchAbility(-1);
        _inputManager.OnSkillSwitchRightPressed += () => TrySwitchAbility(1);
    }

    private void OnDisable()
    {
        if (_inputManager == null)
            return;

        _inputManager.OnSkill3Pressed -= HandleActivatePressed;
    }

    private void HandleActivatePressed()
    {
        if (_activeAbility == null) return;

        // todo restriction check before activate
        if (_playerStats != null && !_playerStats.CheckMana(_activeAbility.ManaCost)) return;

        _playerStats.ConsumeMana(_activeAbility.ManaCost);

        // activate ability via tag
        _abilitySystem.TryActivateAbilityWithTag(_activeAbility.Ability.AbilityTags.Tags[0]);
    }

    public void TrySwitchAbility(int direction)
    {
        if (_abilitySlots.Count <= 1)
        {
            return;
        }
        int currentIndex = _abilitySlots.IndexOf(_activeAbility);
        int nextIndex = (currentIndex + direction + _abilitySlots.Count) % _abilitySlots.Count;
        _activeAbility = _abilitySlots[nextIndex];

        OnActiveAbilityChanged?.Invoke(_activeAbility);
    }

    /// <summary>
    /// Adds a skill to the player's available slots and registers it with the AbilitySystem.
    /// </summary>
    /// for future scene grants ability if certain progression is reached
    public void GrantAbility(AbilityDisplayDataSO newAbilityGranted)
    {
        if (newAbilityGranted == null || _abilitySlots.Contains(newAbilityGranted))
        {
            return;
        }
        _abilitySlots.Add(newAbilityGranted);

        // Set as active if its the only one in slot
        if (_activeAbility == null)
        {
            _activeAbility = newAbilityGranted;
        }

        int newIndex = _abilitySlots.Count - 1;
        _abilitySystem.GiveAbility(newAbilityGranted.Ability);
        OnAbilitySlotChanged?.Invoke(newIndex, newAbilityGranted);
        OnActiveAbilityChanged?.Invoke(_activeAbility);
    }


}
