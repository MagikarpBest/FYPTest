using UnityEngine;

public class JumpState : State
{
    private IHasMovement movement;

    public JumpState(HierarchicalStateMachine stateMachine, ICharacter characterContext, State parent,
        ICharacterStateFactory factory) : base(stateMachine, characterContext, parent, factory)
    {
        movement = characterContext as IHasMovement;
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
        movement.Move();
    }
}
