using UnityEngine;

public class EnemyAttackState : State
{
    private EnemyAttack attackSystem;

    public EnemyAttackState(HierarchicalStateMachine stateMachine, Enemy character, State parent,
        ICharacterStateFactory factory) : base(stateMachine, character, parent, factory)
    {
        attackSystem = character.AttackSystem;
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
