using System;
using UnityEngine;

// Base component
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider))]

// Logic
[RequireComponent(typeof(PlayerAttack))]
[RequireComponent(typeof(PlayerMovementRB))]
[RequireComponent(typeof(PlayerSkillController))]

// others
[RequireComponent(typeof(HierarchicalStateMachine))]
[RequireComponent(typeof(PlayerStatus))]
[RequireComponent(typeof(PlayerDamageReceiver))]
public class Player : MonoBehaviour, ICharacter
{
    public GameObject Ragdoll { get; private set; }
    
    public PlayerInputManager Input { get; private set; }
    public PlayerMovementRB Movement { get; private set; }
    public PlayerAttack AttackSystem { get; private set; }

    public PlayerSkillController SkillController { get; private set; } // Control skill logic
    public PlayerStatus CurrentPlayerStatus { get; private set; } // Player data
    public PlayerDamageReceiver DamageReceiver { get; private set; } // 
    public PlayerModeController ModeController { get; private set; } // Control mode switches
    public PlayerActionRestrictions Restrictions => ModeController.CurrentMode.Restrictions;
    
    private HierarchicalStateMachine stateMachine;
    
    private Collider Collider;
    private Rigidbody Rigidbody;
    private Animator Animator;

    [SerializeField] private Transform model;
    private Transform cameraTransform;

    private void Awake()
    {
        Collider = GetComponent<Collider>();
        Rigidbody = GetComponent<Rigidbody>();
        Animator = GetComponent<Animator>();
        cameraTransform = Camera.main.transform;

        Input = FindFirstObjectByType<PlayerInputManager>();
        Movement = GetComponent<PlayerMovementRB>();
        AttackSystem = GetComponent<PlayerAttack>();

        SkillController = GetComponent<PlayerSkillController>();
        CurrentPlayerStatus = GetComponent<PlayerStatus>();
        DamageReceiver = GetComponent<PlayerDamageReceiver>();
        ModeController = GetComponent<PlayerModeController>();

        Movement.Init(Rigidbody, model, cameraTransform, Animator);
        AttackSystem.Init(Animator);
        
        stateMachine = GetComponent<HierarchicalStateMachine>();
        PlayerStateFactory factory = new PlayerStateFactory(stateMachine, this);

        stateMachine.Init(factory.Alive);
    }
}
