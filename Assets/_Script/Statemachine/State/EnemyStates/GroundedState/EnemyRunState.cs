using UnityEngine;

public class EnemyRunState : State
{
    private EnemyMovement movement;
    private EnemySensor sensor;
    private EnemyAttack attack;
    
    public EnemyRunState(HierarchicalStateMachine stateMachine, Enemy character, State parent,
        ICharacterStateFactory factory)
        : base(stateMachine, character, parent, factory)
    {
        movement = character.Movement;
        sensor = character.Sensor;
    }

    protected override State GetInitialState() => null;

    protected override State GetTransition()
    {
        bool shouldStop = !sensor.HasTarget || sensor.DistanceToTarget <= attack.AttackRange;

        if (shouldStop)
        {
            return Factory.Idle;
        }

        return null;
    }

    protected override void OnEnter()
    {
    }

    protected override void OnExit()
    {
        //movement.StopMove();
    }

    protected override void OnPhysicsTick(float fixedDeltaTime)
    {
        if (sensor.Target != null)
        {
            movement.Move(sensor.Target.position);
        }
    }
}
