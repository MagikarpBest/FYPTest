using System;
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

[RequireComponent(typeof(PlayerStatus))]
public class PlayerSkillController : MonoBehaviour
{
    private PlayerStatus _playerStatus;
    private PlayerInputManager _inputManager;

    [Header("PlayerSkillContext")]
    [SerializeField] private Transform _characterTransform;
    [SerializeField] private Transform _skillHoldPoint;
    [SerializeField] private LineRenderer _aimLineRenderer;

    private PlayerSkillContext _context;
    private PlayerSkill _cachedSkill;
    private IPlayerSkillHandler _activeHandler;
    
    // for state machine if using certain skill cant do certain action
    public PlayerActionRestrictions CurrentRestrictions => _activeHandler?.GetRestrictions() ?? PlayerActionRestrictions.None;

    public bool IsSkillActive => _activeHandler != null;

    private void Awake()
    {
        _playerStatus = GetComponent<PlayerStatus>();
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
        if (_playerStatus == null) return;
        PlayerSkill activeSkill = _playerStatus.ActiveSkill;
        if (activeSkill == null) return;

        if (_playerStatus.CheckMana(activeSkill.ManaCost) == false) return;
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

        _activeHandler.Begin(_context);
        Debug.Log("SelectSkill Success");
    }

    public void ConfirmSkill()
    {
        if (_activeHandler == null) return;

        SkillConfirmResult result = _activeHandler.Confirm();

        if (result == SkillConfirmResult.Finish) FinishSkill();
    }

    private void Update()
    {
        _activeHandler?.Update();
    }

    private void CancelCurrentSkill()
    {
        _activeHandler?.Cancel();
        _activeHandler = null;
    }

    private void FinishSkill()
    {
        _playerStatus.ConsumeMana(_cachedSkill.ManaCost);
        _activeHandler?.Cancel();

        _activeHandler = null;
    }
}