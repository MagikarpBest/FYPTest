using UnityEngine;

public class EnemyDeadState : State
{
    private Enemy enemy;
    
    public EnemyDeadState(HierarchicalStateMachine stateMachine, Enemy enemy, State parent,
        ICharacterStateFactory factory) : base(stateMachine, enemy, parent, factory)
    {
        this.enemy = enemy;
    }
    
    protected override void OnEnter()
    {
        Debug.Log("Dead");
        enemy.EnableRagdoll();
    }
}
