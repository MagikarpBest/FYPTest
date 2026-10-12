using UnityEngine;

public class EnemyAttackState : HSMState
{
    private EnemyAttack attackSystem;
    private IStateContainer AllStates;
    public EnemyAttackState(HierarchicalStateMachine stateMachine, Enemy character, HSMState parent,
        IStateContainer factory) : base(stateMachine, parent)
    {
        attackSystem = character.AttackSystem;
        AllStates = factory;
    }

    protected override HSMState GetInitialState() => AllStates.BaseAttack;

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
