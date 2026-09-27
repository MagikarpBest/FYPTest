using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider))]

[RequireComponent(typeof(PlayerMovementRB))]
[RequireComponent(typeof(HierarchicalStateMachine))]

[RequireComponent(typeof(PlayerSkillController))]
[RequireComponent(typeof(PlayerStatus))]
[RequireComponent(typeof(PlayerDamageReceiver))]


public class Player : MonoBehaviour, ICharacter
{
    public PlayerInputManager Input {get; private set;}
    public PlayerMovementRB Movement {get; private set;}
    
    public PlayerSkillController SkillController {get; private set;}    // Control skill logic
    public PlayerStatus CurrentPlayerStatus {get; private set;}         // Player data
    public PlayerDamageReceiver DamageReceiver {get; private set;}      // 
    public PlayerModeController ModeController {get; private set;}      // Control mode switches
    
    private HierarchicalStateMachine stateMachine;
    public float Health { get; set; } = 100f; // Fake health
    public bool IsDead => Health <= 0;
    
    //I like having all the stuff the shared components need here so u just pass it to the compoenents rather than having to assign or use get compoenent in all the individual scripts
    //get component is expensive so if u can reduce its best
    //and assigning alot of serialize field is also annoying
    
    public Collider Collider {get; private set;}
    public Rigidbody Rigidbody {get; private set;}
    public Animator Animator {get; private set;}
    
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

        SkillController = GetComponent<PlayerSkillController>();
        CurrentPlayerStatus = GetComponent<PlayerStatus>();
        DamageReceiver = GetComponent<PlayerDamageReceiver>();
        ModeController = GetComponent<PlayerModeController>();
        
        //all these components can be pure c sharp but then u cant see them in inspector
        
        Movement.Init(Rigidbody, model, cameraTransform, Animator);
        
        //u actually want a root state which substates are alive and dead 
        stateMachine = GetComponent<HierarchicalStateMachine>();
        PlayerStateFactory factory = new PlayerStateFactory(stateMachine, this);
        stateMachine.Init(factory.Alive);
    }
}

