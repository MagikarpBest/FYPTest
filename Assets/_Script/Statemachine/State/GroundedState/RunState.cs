using UnityEngine;

public class RunState : State
{
    private Player player;
    private PlayerMovementRB movement;
    private PlayerInputManager input;

    public RunState(HierarchicalStateMachine stateMachine, ICharacter characterContext, State parent,
        PlayerStateFactory factory) 
        : base(stateMachine, characterContext, parent, factory)
    
    {
        if (CharacterContext is Player player)
        {
            this.player = player;
            movement = player.Movement;
            input = player.Input;
        }
    }

    protected override State GetInitialState() => null;

    protected override State GetTransition()
    {
        if (input.MoveInput == Vector2.zero)
        {
            return Factory.Idle;
        }

        return null;
    }

    protected override void OnEnter()
    {
        movement.StopMove();
    }

    protected override void OnPhysicsTick(float fixedDeltaTime)
    {
        if (player.ModeController.CurrentMode.Restrictions.HasFlag(PlayerActionRestrictions.RestrictMovement))
        {
            movement.StopMove();
            return;
        }
        movement.Move(input.MoveInput);
        movement.RotateTowardsMovement();
    }
}
