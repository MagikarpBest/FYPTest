using UnityEngine;

public class EnemyAliveState : State
{
    private EnemyStats _stats;
    private Enemy enemy;
    
    public EnemyAliveState(HierarchicalStateMachine stateMachine, Enemy enemy, State parent,
        ICharacterStateFactory factory) : base(stateMachine, enemy, parent, factory)
    {
        _stats = enemy.Stats;
        this.enemy = enemy;
    }

    protected override State GetInitialState() => Factory.Grounded;

    protected override State GetTransition()
    {
        if (_stats.CurrentHealth <= 0)
        {
            return Factory.Dead;
        }
        return null;
    }
    
    protected override void OnEnter()
    {
        enemy.DisableRagdoll();
    }
}
