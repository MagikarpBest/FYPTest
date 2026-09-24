using System;
using UnityEngine;

public class PlayerStateMachine :MonoBehaviour
{
    // get
    public PlayerInputManager Input { get; private set; }
    public PlayerMovementRB Movement { get; private set; }
    
    // top level state only, sub state check its parent state.cs
    public GroundedState Grounded { get; private set; }
    public AirborneState Airborne { get; private set; }
    private PlayerState currentState;


    
    private void Awake()
    {
        Input = FindFirstObjectByType<PlayerInputManager>();
        Movement = FindFirstObjectByType<PlayerMovementRB>();
        // create top level states
        Grounded = new GroundedState(this);
        Airborne = new AirborneState(this);

        
        Input.OnJumpPressed += HandleJump;
        Movement.OnLaunched += HandleLaunch;
    }
    
    private void OnDestroy()
    {
        Input.OnJumpPressed -= HandleJump;
        Movement.OnLaunched -= HandleLaunch;
    }
    
    private void HandleLaunch()
    {
        currentState?.HandleLaunch();
    }
    private void HandleJump()
    {
        currentState?.HandleJump();
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
