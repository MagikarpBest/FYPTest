

public class AirborneState : State
{
    public readonly JumpState Jump;
    public readonly FallState Fall;
    public readonly LaunchState Launch;
    
    private PlayerMovementRB movement;
    private PlayerInputManager input;
    
    public AirborneState(HierarchicalStateMachine stateMachine, ICharacter characterContext, State parent) : base(stateMachine, characterContext, parent)
    {
        Jump = new JumpState(stateMachine, characterContext, this);
        Fall = new FallState(stateMachine, characterContext, this);
        Launch = new LaunchState(stateMachine, characterContext, this);
        
        if (CharacterContext is Player player)
        {
            movement = player.Movement;
            input = player.Input;
        }
    }
    
    protected override State GetInitialState() => Fall;
    
    private void HandleLaunch() => EventRequestTransition(Launch);
    
    protected override State GetTransition()
    {
        if (eventRequestedTransition != null) return eventRequestedTransition;

        return null;
    }

    protected override void OnEnter()
    {
        movement.OnLaunched += HandleLaunch;
    }

    protected override void OnExit()
    {
        movement.OnLaunched -= HandleLaunch;
    }

    protected override void OnTick(float deltaTime)
    {
        
    }

    protected override void OnPhysicsTick(float fixedDeltaTime)
    {

    }
}
