using UnityEngine;

public class JumpState : State
{
    private PlayerMovementRB movement;
    private PlayerInputManager input;

    public JumpState(HierarchicalStateMachine stateMachine, Player player, State parent,
        ICharacterStateFactory factory) : base(stateMachine, player, parent, factory)
    {
        movement = player.Movement;
        input = player.Input;
    }

    protected override State GetInitialState() => null;

    protected override State GetTransition()
    {
        if (movement.IsFalling)
        {
            return Factory.Fall;
        } //at apex 

        return null;
    }

    protected override void OnEnter()
    {
        movement.Jump();
    }

    protected override void OnPhysicsTick(float fixedDeltaTime)
    {
        movement.Move(input.MoveInput);
    }
}
