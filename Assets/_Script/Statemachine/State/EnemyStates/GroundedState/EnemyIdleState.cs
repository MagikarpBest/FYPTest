using UnityEngine;

public class EnemyIdleState : State
{
    private EnemyMovement movement;

    public EnemyIdleState(HierarchicalStateMachine stateMachine, Enemy enemy, State parent, 
        ICharacterStateFactory factory) : base(stateMachine, enemy, parent, factory)
    {
        movement = enemy.Movement;
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
