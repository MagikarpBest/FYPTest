using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider))]

[RequireComponent(typeof(PlayerInputManager))]
[RequireComponent(typeof(PlayerMovementRB))]
[RequireComponent(typeof(HierarchicalStateMachine))]

[RequireComponent(typeof(PlayerBombSkill))]
[RequireComponent(typeof(PlayerCreateSkill))]

public class Player : MonoBehaviour, ICharacter
{
    public PlayerInputManager Input {get; private set;}
    public PlayerMovementRB Movement {get; private set;}
    
    //Make these a PlayerSkillManager or something if u have more skill later if only these two or like 3 ish skills its fine this way
    public PlayerBombSkill BombSkill {get; private set;}
    public PlayerCreateSkill CreateSkill {get; private set;}
    
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
        
        Input = GetComponent<PlayerInputManager>();
        Movement = GetComponent<PlayerMovementRB>();

        BombSkill = GetComponent<PlayerBombSkill>();
        CreateSkill = GetComponent<PlayerCreateSkill>();
        
        //all these components can be pure c sharp but then u cant see them in inspector
        
        Movement.Init(Rigidbody, model, cameraTransform, Animator);
        
        //u actually want a root state which substates are alive and dead 
        stateMachine = GetComponent<HierarchicalStateMachine>();
        PlayerStateFactory factory = new PlayerStateFactory(stateMachine, this);
        stateMachine.Init(factory.Alive);
    }
}
