using UnityEngine;

public class EnemyIdleState : State
{
    private EnemyMovement movement;

    public EnemyIdleState(HierarchicalStateMachine stateMachine, Enemy character, State parent, 
        ICharacterStateFactory factory) : base(stateMachine, character, parent, factory)
    {
        movement = character.Movement;
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
