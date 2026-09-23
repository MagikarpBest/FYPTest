using System;
using UnityEngine;

public class PlayerStateMachine :MonoBehaviour
{
    // get
    public PlayerInputManager Input { get; private set; }
    public PlayerMovementRB Movement { get; private set; }
    
    // Top level
    public GroundedState Grounded { get; private set; }
    public AirborneState Airborne { get; private set; }

    // Grounded children
    public IdleState Idle { get; private set; }
    public RunState Run { get; private set; }

    // Air children
    public JumpState Jump { get; private set; }
    public FallState Fall { get; private set; }
    public LaunchState Launch { get; private set; }
    
    
    private PlayerState currentState;


    
    private void Awake()
    {
        Input = FindFirstObjectByType<PlayerInputManager>();
        Movement = FindFirstObjectByType<PlayerMovementRB>();
        Movement.OnLaunched += HandleLaunch;
        Input.OnJumpPressed += HandleJump;

        
        // create parents

        Grounded = new GroundedState(this);
        Airborne = new AirborneState(this);
        
        // create children
        Idle = new IdleState(this);
        Run = new RunState(this);


        Jump = new JumpState(this);
        Fall = new FallState(this);
        Launch = new LaunchState(this);
    }

    private void HandleJump()
    {
        currentState?.HandleJump();
    }
    

    private void HandleLaunch(Vector3 force)
    {
        ChangeState(Airborne);

        Airborne.ChangeChild(Launch);
    }

    private void Start()
    {
        ChangeState(Grounded);
    }

    public void ChangeState(PlayerState newState)
    {
        Debug.Log($"<color=blue>State change: {currentState} -> {newState}<color=blue>");
        currentState?.Exit();

        currentState = newState;
        
        currentState?.Enter();
    }
    
    public void Update()
    {
        currentState?.Update();
    }


    public void FixedUpdate()
    {
        currentState?.FixedUpdate();
    }
}
