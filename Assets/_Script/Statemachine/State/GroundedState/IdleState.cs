using UnityEngine;

public class IdleState : State
{
    private IHasMovement movement;
    private IHasInput input;

    public IdleState(HierarchicalStateMachine stateMachine, ICharacter characterContext, State parent,
        ICharacterStateFactory factory) : base(stateMachine, characterContext, parent, factory)
    {
        movement = characterContext as IHasMovement;
        input = characterContext as IHasInput;

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
