using System;
using System.Collections;
using UnityEngine;

public enum SkillConfirmResult
{
    Finish = 1,
    Proceed = 2, // If skill need to confirm more than 1 steps.
    Fail = -1
}

/// <summary>
/// Packed {CharacterTransform, SkillHoldPoint, Camera} reference sent from PlayerSkillController.cs
/// </summary>
public class PlayerSkillContext
{
    public Transform CharacterTransform { get; }
    public Transform SkillHoldPoint { get; }
    public Camera Camera { get; }
    public LineRenderer AimLineRenderer { get; }

    public PlayerSkillContext(
        Transform characterTransform,
        Transform skillHoldPoint,
        Camera camera,
        LineRenderer aimLineRenderer)
    {
        CharacterTransform = characterTransform;
        SkillHoldPoint = skillHoldPoint;
        Camera = camera;
        AimLineRenderer = aimLineRenderer;
    }
}

[RequireComponent(typeof(PlayerStats))]
public class PlayerSkillController : MonoBehaviour
{
    private PlayerStats _playerStats;
    private PlayerInputManager _inputManager;

    [Header("PlayerSkillContext")]
    [SerializeField] private Transform _characterTransform;
    [SerializeField] private Transform _skillHoldPoint;
    [SerializeField] private LineRenderer _aimLineRenderer;

    private PlayerModeController _modeController;
    private PlayerSkillContext _context;
    private PlayerSkill _cachedSkill;
    private IPlayerSkillHandler _activeHandler;



    public bool IsSkillUsable { get; private set; } = true;

    private void Awake()
    {
        _playerStats = GetComponent<PlayerStats>();
        _modeController = GetComponent<PlayerModeController>();
        _context = new PlayerSkillContext(_characterTransform, _skillHoldPoint, Camera.main, _aimLineRenderer);
    }

    private void Start()
    {
        FindInputManager();
        BindInputEvents();
    }

    private void OnDisable()
    {
        UnbindInputEvents();
    }

    private void FindInputManager()
    {
        _inputManager = FindFirstObjectByType<PlayerInputManager>();
    }

    private void BindInputEvents()
    {
        if (_inputManager == null)
        {
            Debug.LogWarning($"{nameof(PlayerSkillController)}: " + $"{nameof(PlayerInputManager)} not found.", this);
            return;
        }

        _inputManager.OnSkill3Pressed += HandleSkillPressed;
        _inputManager.OnAttackPressed += HandleAttackPressed;
        _inputManager.OnSkillCancelPressed += CancelCurrentSkill;
    }

    private void UnbindInputEvents()
    {
        if (_inputManager == null)
            return;

        _inputManager.OnSkill3Pressed -= HandleSkillPressed;
        _inputManager.OnAttackPressed -= HandleAttackPressed;
        _inputManager.OnSkillCancelPressed -= CancelCurrentSkill;
    }

    private void HandleSkillPressed()
    {
        //if (_modeController.CurrentMode.Restrictions.HasFlag(PlayerActionRestrictions.RestrictSkill)) return;
        if (!IsSkillUsable)
        {
            Debug.Log("Skill not usable");
            return;
        }
        if (_playerStats == null) return;
        PlayerSkill activeSkill = _playerStats.ActiveSkill;
        if (activeSkill == null) return;

        if (_playerStats.CheckMana(activeSkill.ManaCost) == false) return;
        
        SelectSkill(activeSkill);
    }

    private void HandleAttackPressed()
    {
        ConfirmSkill();
    }

    public void SelectSkill(PlayerSkill skill)
    {
        if (skill == null)
            return;

        CancelCurrentSkill();

        _cachedSkill = skill;

        _activeHandler = skill.Handler;

        if (_activeHandler == null)
            return;
        _modeController.ChangeMode(_modeController.Ability);
        _activeHandler.Begin(_context);
        Debug.Log("SelectSkill Success");
    }

    public void ConfirmSkill()
    {
        if (_activeHandler == null) return;

        SkillConfirmResult result = _activeHandler.Confirm();
        if (result == SkillConfirmResult.Finish) StartCoroutine(FinishSkill());

    }

    private void Update()
    {
        
        _activeHandler?.Update();
    }

    private void CancelCurrentSkill()
    {
        _activeHandler?.Cancel();
        _activeHandler = null;
        _cachedSkill = null;
        _modeController.ChangeMode(_modeController.Normal);
    }

    private IEnumerator FinishSkill()
    {
        _playerStats.ConsumeMana(_cachedSkill.ManaCost);
        _activeHandler?.Cancel();
        _activeHandler = null;
        _cachedSkill = null;
        
        // Delay 0.5 so left click doesnt trigger attack at same frame
        yield return new WaitForSeconds(0.2f);
        _modeController.ChangeMode(_modeController.Normal);
    }
    
    // state machine related stuff
    public void SetSkillUsable(bool value)
    {
        if (IsSkillUsable == value)
        {
            return;
        }

        IsSkillUsable = value;

        if (!value && _activeHandler != null)
        {
            CancelCurrentSkill();
        }
    }
}