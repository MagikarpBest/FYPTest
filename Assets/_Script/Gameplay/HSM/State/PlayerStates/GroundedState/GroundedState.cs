using UnityEngine;

public class GroundedState : HSMState
{
    private PlayerMovementRB movement;
    private PlayerAttack attackSystem;
    private PlayerInputManager input;
    private IStateContainer AllStates;
    private Player Character;

    public GroundedState(HierarchicalStateMachine stateMachine, Player player, HSMState parent,
        IStateContainer factory) : base(stateMachine, parent)
    {
        movement = player.Movement;
        attackSystem = player.AttackSystem;
        input = player.Input;
        AllStates = factory;
        Character = player;
    }

    // Use the factory to set the initial child
    protected override HSMState GetInitialState()
    {
        if (input.MoveInput != Vector2.zero)
        {
            return AllStates.Run;
        }
        else
        {
            return AllStates.Idle;
        }
    }

    // test restriction
    private void HandleJump()
    {
        // Check if jumping is restricted
        if (Character.Restrictions.HasFlag(PlayerActionRestrictions.RestrictJump))
        {
            return;
        }

        EventRequestTransition(AllStates.Jump);
    }

    private void HandleAttack()
    {
        // Check if attacking is restricted before allowing the transition
        if (Character.Restrictions.HasFlag(PlayerActionRestrictions.RestrictAttack))
        {
            return;
        }
        EventRequestTransition(AllStates.Attack);
    }

    // todo if left click pressed and not skill mode change to attack

    protected override HSMState GetTransition()
    {
        if (eventRequestedTransition != null)
        {
            return eventRequestedTransition;
        }

        if (!movement.IsGrounded)
        {
            return AllStates.Airborne;
        }
        return null;
    }

    protected override void OnEnter()
    {
        input.OnJumpPressed += HandleJump;
        //input.OnAttackPressed += HandleAttack;
    }


    protected override void OnExit()
    {
        input.OnJumpPressed -= HandleJump;
        //input.OnAttackPressed -= HandleAttack;
    }

}
