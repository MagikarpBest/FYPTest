using UnityEngine;

public class IdleState : State
{
    private PlayerMovementRB movement;
    private PlayerInputManager input;

    public IdleState(HierarchicalStateMachine stateMachine, Player player, State parent,
        ICharacterStateFactory factory) : base(stateMachine, player, parent, factory)
    {
        movement = player.Movement;
        input = player.Input;
    }

    protected override State GetInitialState() => null;

    protected override State GetTransition()
    {
        if (movement.IsMoving)
        {
            return Factory.Run;
        }

        return null;
    }

    protected override void OnEnter()
    {
        movement.StopMove();
    }
}
