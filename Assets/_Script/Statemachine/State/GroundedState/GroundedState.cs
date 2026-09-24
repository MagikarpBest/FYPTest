
public class GroundedState : State
{
    public readonly IdleState Idle;
    public readonly RunState Run;
    
    private PlayerMovementRB movement;
    private PlayerInputManager input;
    
    public GroundedState(HierarchicalStateMachine stateMachine, ICharacter characterContext, State parent) : base(stateMachine, characterContext, parent)
    {
        Idle = new IdleState(stateMachine, characterContext, this);
        Run = new RunState(stateMachine, characterContext, this);
        
        if (CharacterContext is Player player)
        {
            movement = player.Movement;
            input = player.Input;
        }
    }
    
    protected override State GetInitialState() => Idle;

    private void HandleJump() => EventRequestTransition(((AliveState)Parent).Airborne.Jump);
    
    protected override State GetTransition()
    {
        if (eventRequestedTransition != null) return eventRequestedTransition;
        
        if (!movement.IsGrounded) return ((AliveState)Parent).Airborne;

        return null;
    }
    
    protected override void OnEnter()
    {
        input.OnJumpPressed += HandleJump;
    }

    protected override void OnExit()
    {
        input.OnJumpPressed -= HandleJump;
    }

}
