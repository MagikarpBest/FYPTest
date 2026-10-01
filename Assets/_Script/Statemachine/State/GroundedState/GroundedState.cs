public class GroundedState : State
{
    private IHasMovement movement;
    private IHasInput input;

    public GroundedState(HierarchicalStateMachine stateMachine, ICharacter characterContext, State parent,
        ICharacterStateFactory factory) : base(stateMachine, characterContext, parent, factory)
    {
        movement = characterContext as IHasMovement;
        input = characterContext as IHasInput;
    }

    // Use the factory to set the initial child
    protected override State GetInitialState()
    {
        if(movement.IsMoving)
        {
            return Factory.Run;
        }
        else
        {
            return Factory.Idle;
        }
    } 

    // test restriction
    private void HandleJump()
    {
        // // if current mode is unjumpable then dont let
        // if (player.ModeController.CurrentMode.Restrictions.HasFlag(PlayerActionRestrictions.RestrictJump))
        // {
        //     return;
        // }

        EventRequestTransition(Factory.Jump);
    } 
    private void HandleAttack()
    {
        EventRequestTransition(Factory.Attack);
    }

    // todo if left click pressed and not skill mode change to attack

    protected override State GetTransition()
    {
        if (eventRequestedTransition != null)
        {
            return eventRequestedTransition;
        }
        if(!movement.IsGrounded)
        {
            return Factory.Airborne;
        }
        return null;
    }

    protected override void OnEnter()
    {
        if(input!=null)
        {
            input.OnJumpPressed += HandleJump;
            input.OnAttackPressed += HandleAttack;
        }
    }



    protected override void OnExit()
    {
        if (input != null)
        {
            input.OnJumpPressed -= HandleJump;
            input.OnAttackPressed -= HandleAttack;
        }
    }

}
