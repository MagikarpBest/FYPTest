public class GroundedState : State
{
    // move to factory
    // public readonly IdleState Idle;
    // public readonly RunState Run;

    private PlayerMovementRB movement;
    private PlayerInputManager input;

    public GroundedState(HierarchicalStateMachine stateMachine, ICharacter characterContext, State parent,
        PlayerStateFactory factory) : base(stateMachine, characterContext, parent, factory)
    {

        if (CharacterContext is Player player)
        {
            movement = player.Movement;
            input = player.Input;
        }
    }

    // Use the factory to set the initial child
    protected override State GetInitialState() => Factory.Idle;

    private void HandleJump() => EventRequestTransition(Factory.Jump);

    protected override State GetTransition()
    {
        if (eventRequestedTransition != null)
        {
            return eventRequestedTransition;
        }

        if (!movement.IsGrounded)
        {
            return Factory.Airborne;
        }

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
