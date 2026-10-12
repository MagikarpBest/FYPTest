using UnityEngine;

public class EnemyGroundedState : HSMState
{
    private EnemyMovement movement;
    private EnemyAttack attackSystem;
    private EnemySensor sensor;
    private IStateContainer AllStates;
    public EnemyGroundedState(HierarchicalStateMachine stateMachine, Enemy character, HSMState parent,
        IStateContainer factory) : base(stateMachine, parent)
    {
        movement = character.Movement;
        attackSystem = character.AttackSystem;
        sensor = character.Sensor;
        AllStates = factory;
        
    }
    

    protected override HSMState GetTransition()
    {
        if (eventRequestedTransition != null)
        {
            return eventRequestedTransition;
        }

        bool canAttack = sensor.HasTarget && sensor.DistanceToTarget <= attackSystem.AttackRange;

        if (canAttack)
        {
            return AllStates.Attack;
        }

        if (!movement.IsGrounded)
        {
            return AllStates.Airborne;
        }
        bool shouldMove = sensor.HasTarget && sensor.DistanceToTarget > attackSystem.AttackRange;
        if (shouldMove)
        {
            return AllStates.Run;

        }
        else
        {
            return AllStates.Idle;
        }
        return null;
    }

    protected override void OnEnter()
    {
    }


    protected override void OnExit()
    {
    }

}
