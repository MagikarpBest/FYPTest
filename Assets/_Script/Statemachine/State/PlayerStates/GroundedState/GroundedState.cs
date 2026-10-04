public class GroundedState : State
{
    private PlayerMovementRB movement;
    private PlayerAttack attackSystem;
    private PlayerInputManager input;

    public GroundedState(HierarchicalStateMachine stateMachine, Player enemy, State parent,
        ICharacterStateFactory factory) : base(stateMachine, enemy, parent, factory)
    {
        movement = enemy.Movement;
        attackSystem = enemy.AttackSystem;
        input = enemy.Input;
    }

    // Use the factory to set the initial child
    protected override State GetInitialState()
    {
        if (movement.IsMoving)
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
        // Check if jumping is restricted
        if (Enemy.Restrictions.HasFlag(PlayerActionRestrictions.RestrictJump))
        {
            return;
        }

        EventRequestTransition(Factory.Jump);
    }

    private void HandleAttack()
    {
        // Check if attacking is restricted before allowing the transition
        if (Enemy.Restrictions.HasFlag(PlayerActionRestrictions.RestrictAttack))
        {
            return;
        }
        EventRequestTransition(Factory.Attack);
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
        input.OnAttackPressed += HandleAttack;
    }


    protected override void OnExit()
    {
        input.OnJumpPressed -= HandleJump;
        input.OnAttackPressed -= HandleAttack;
    }

}
