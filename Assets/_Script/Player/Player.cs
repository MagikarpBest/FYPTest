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
public class Player : MonoBehaviour, ICharacter, IHasMovement, IHasAttack, IHasInput,ICanUseSkills
{
    public PlayerInputManager Input { get; private set; }
    public PlayerMovementRB Movement { get; private set; }
    public PlayerAttack AttackSystem { get; private set; }

    public PlayerSkillController SkillController { get; private set; } // Control skill logic
    public PlayerStatus CurrentPlayerStatus { get; private set; } // Player data
    public PlayerDamageReceiver DamageReceiver { get; private set; } // 
    public PlayerModeController ModeController { get; private set; } // Control mode switches

    private HierarchicalStateMachine stateMachine;

    // TEST
    // Implementation of state interface so reusable
    bool IHasMovement.IsGrounded => Movement.IsGrounded;
    bool IHasMovement.IsMoving => Input.MoveInput != Vector2.zero;
    void IHasMovement.Move()
    {
        Movement.Move(Input.MoveInput);
        Movement.RotateTowardsMovement();
    }
    void IHasMovement.StopMove() => Movement.StopMove();

    bool IHasAttack.isAttacking => AttackSystem.isAttacking;
    void IHasAttack.HandleAtack() => AttackSystem.HandleAtack();

    void ICanUseSkills.SetSkillUsable(bool usable) => SkillController.SetSkillUsable(usable);
    
    public event Action OnJumpPressed { add => Input.OnJumpPressed += value; remove => Input.OnJumpPressed -= value; }
    public event Action OnAttackPressed { add => Input.OnAttackPressed += value; remove => Input.OnAttackPressed -= value; }

    //I like having all the stuff the shared components need here so u just pass it to the compoenents rather than having to assign or use get compoenent in all the individual scripts
    //get component is expensive so if u can reduce its best
    //and assigning alot of serialize field is also annoying

    public Collider Collider { get; private set; }
    public Rigidbody Rigidbody { get; private set; }
    public Animator Animator { get; private set; }

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

        //all these components can be pure c sharp but then u cant see them in inspector

        Movement.Init(Rigidbody, model, cameraTransform, Animator);
        AttackSystem.Init(Animator);

        //u actually want a root state which substates are alive and dead 
        stateMachine = GetComponent<HierarchicalStateMachine>();
        PlayerStateFactory factory = new PlayerStateFactory(stateMachine, this);
        stateMachine.Init(factory.Alive);
    }
}
