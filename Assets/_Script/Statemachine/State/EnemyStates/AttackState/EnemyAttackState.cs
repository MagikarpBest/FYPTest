using UnityEngine;

public class EnemyAttackState : State
{
    private EnemyAttack attackSystem;

    public EnemyAttackState(HierarchicalStateMachine stateMachine, Enemy enemy, State parent,
        ICharacterStateFactory factory) : base(stateMachine, enemy, parent, factory)
    {
        attackSystem = enemy.AttackSystem;
    }

    protected override State GetInitialState() => Factory.BaseAttack;

    protected override void OnEnter()
    {
    }

    protected override void OnExit()
    {
    }
    private void HandleAttack()
    {
        //attackSystem.RequestAttack();
    }
}
