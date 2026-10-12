using UnityEngine;

public class EnemyDeadState : HSMState
{
    private Enemy enemy;
    private EnemyCombatReaction reaction;
    
    public EnemyDeadState(HierarchicalStateMachine stateMachine, Enemy enemy, HSMState parent,
        IStateContainer factory) : base(stateMachine, parent)
    {
        this.enemy = enemy;
        reaction = enemy.CombatReaction;
    }
    
    protected override void OnEnter()
    {
        Debug.Log("Dead");
        reaction.DeadAnimation();
        enemy.EnableRagdoll();
    }
}
