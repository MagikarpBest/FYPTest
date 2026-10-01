using UnityEngine;

public class RunState : State
{
    private IHasMovement movement;
    private IHasInput input;


    public RunState(HierarchicalStateMachine stateMachine, ICharacter characterContext, State parent,
        ICharacterStateFactory factory)
        : base(stateMachine, characterContext, parent, factory)

    {
        movement = characterContext as IHasMovement;
        input = characterContext as IHasInput;
    }

    protected override State GetInitialState() => null;

    protected override State GetTransition()
    {
        if (!movement.IsMoving)
        {
            return Factory.Idle;
        }

        return null;
    }

    protected override void OnEnter()
    {
    }

    protected override void OnExit()
    {
        //movement.StopMove();
    }

    protected override void OnPhysicsTick(float fixedDeltaTime)
    {
        movement.Move();
    }
}
