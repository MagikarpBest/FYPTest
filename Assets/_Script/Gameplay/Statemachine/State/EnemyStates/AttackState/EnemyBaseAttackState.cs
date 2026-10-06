using UnityEngine;

public class EnemyBaseAttackState : State
{
    private EnemyAttack attackSystem;
    private EnemyMovement movement;

    public EnemyBaseAttackState(HierarchicalStateMachine stateMachine, Enemy character, State parent,
        ICharacterStateFactory factory) : base(stateMachine, character, parent, factory)
    {
        attackSystem = character.AttackSystem;
        movement = character.Movement;
    }

    protected override State GetTransition()
    {
        if (!attackSystem.IsAttacking)
        {
            return Factory.Grounded;
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
