using UnityEngine;

public class EnemyDeadState : State
{
    private Enemy enemy;
    private EnemyCombatReaction reaction;
    
    public EnemyDeadState(HierarchicalStateMachine stateMachine, Enemy enemy, State parent,
        ICharacterStateFactory factory) : base(stateMachine, enemy, parent, factory)
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
