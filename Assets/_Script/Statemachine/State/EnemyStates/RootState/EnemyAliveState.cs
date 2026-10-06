using UnityEngine;

public class EnemyAliveState : State
{
    private EnemyStatus status;
    
    public EnemyAliveState(HierarchicalStateMachine stateMachine, Enemy enemy, State parent,
        ICharacterStateFactory factory) : base(stateMachine, enemy, parent, factory)
    {
        status = enemy.Status;
    }

    protected override State GetInitialState() => Factory.Grounded;

    protected override State GetTransition()
    {
        if (status.CurrentHealth <= 0)
        {
            return Factory.Dead;
        }
        return null;
    }
}
