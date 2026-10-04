using UnityEngine;

public class EnemyGroundedState : State
{
    private EnemyMovement movement;
    private EnemyAttack attackSystem;
    private EnemySensor sensor;

    public EnemyGroundedState(HierarchicalStateMachine stateMachine, Enemy character, State parent,
        ICharacterStateFactory factory) : base(stateMachine, character, parent, factory)
    {
        movement = character.Movement;
        attackSystem = character.AttackSystem;
        sensor = character.Sensor;
    }
    
    protected override State GetInitialState()
    {
        if (movement.IsMoving)
        {
            return Factory.Run;
        }
        else
        {
            return Factory.Idle;
        }
    }
    
    protected override State GetTransition()
    {
        if (eventRequestedTransition != null)
        {
            return eventRequestedTransition;
        }
        
        bool canAttack = sensor.HasTarget && sensor.DistanceToTarget <= attackSystem.AttackRange;
        if (canAttack)
        {
            return Factory.Attack;
        }

        if (!movement.IsGrounded)
        {
            return Factory.Airborne;
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
