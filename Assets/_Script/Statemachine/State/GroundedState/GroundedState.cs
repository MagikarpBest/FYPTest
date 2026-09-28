public class GroundedState : State
{
    private Player player;
    private PlayerMovementRB movement;
    private PlayerInputManager input;

    public GroundedState(HierarchicalStateMachine stateMachine, ICharacter characterContext, State parent,
        PlayerStateFactory factory) : base(stateMachine, characterContext, parent, factory)
    {

        if (CharacterContext is Player player)
        {
            this.player = player;
            movement = player.Movement;
            input = player.Input;
        }
    }

    // Use the factory to set the initial child
    protected override State GetInitialState() => Factory.Idle;

    // test restriction
    private void HandleJump()
    {
        // if current mode is unjumpable then dont let
        if (player.ModeController.CurrentMode.Restrictions.HasFlag(PlayerActionRestrictions.RestrictJump))
        {
            return;
        }

        EventRequestTransition(Factory.Jump);
    } 

    // todo if left click pressed and not skill mode change to attack

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
        //player.SkillController.SetSkillUsable(false);

    }

    protected override void OnExit()
    {
        input.OnJumpPressed -= HandleJump;
        //player.SkillController.SetSkillUsable(true);

    }

}
