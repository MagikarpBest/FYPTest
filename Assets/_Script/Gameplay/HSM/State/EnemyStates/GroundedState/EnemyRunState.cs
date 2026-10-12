using UnityEngine;

public class EnemyRunState : HSMState
{
    private EnemyMovement movement;
    private EnemySensor sensor;
    private EnemyAttack attack;
    private IStateContainer AllStates;
    public EnemyRunState(HierarchicalStateMachine stateMachine, Enemy character, HSMState parent,
        IStateContainer factory)
        : base(stateMachine, parent)
    {
        movement = character.Movement;
        sensor = character.Sensor;
        attack = character.AttackSystem;
        AllStates = factory;
    }

    protected override HSMState GetInitialState() => null;

    protected override HSMState GetTransition()
    {
        bool shouldStop = !sensor.HasTarget || sensor.DistanceToTarget <= attack.AttackRange;

        if (shouldStop)
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
        //movement.StopMove();
    }

    protected override void OnTick(float deltaTime)
    {
        //Debug.Log("running");
    }

    protected override void OnPhysicsTick(float fixedDeltaTime)
    {
        if (sensor.Target != null)
        {
            movement.Move(sensor.Target.position);
        }
    }
}
