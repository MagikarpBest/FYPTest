using UnityEngine;

public class EnemyBaseAttackState : HSMState
{
    private EnemyAttack attackSystem;
    private EnemyMovement movement;
    private IStateContainer AllStates;
    public EnemyBaseAttackState(HierarchicalStateMachine stateMachine, Enemy character, HSMState parent,
        IStateContainer factory) : base(stateMachine, parent)
    {
        attackSystem = character.AttackSystem;
        movement = character.Movement;
        AllStates = factory;
    }

    protected override HSMState GetTransition()
    {
        if (!attackSystem.IsAttacking)
        {
            return AllStates.Grounded;
        }
        
        return null;
    }

    protected override void OnEnter()
    {        
        // theres some enter bug fix later

        //Debug.Log("On base attackstate enter ");
        movement.StopMove();
        
        // If we are entering this state and we have a queue, it means we are continuing a combo
        if(!attackSystem.IsAttacking)
        {
            //Debug.Log("On base attackstate enter and attack");
            attackSystem.RequestAttack();
        }
    }

    protected override void OnExit()
    {
    }
}
