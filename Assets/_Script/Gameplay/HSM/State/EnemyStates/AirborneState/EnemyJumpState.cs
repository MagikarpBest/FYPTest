using UnityEngine;

public class EnemyJumpState : HSMState
{
    private EnemyMovement movement;
    private EnemySensor sensor;
    private IStateContainer AllStates;
    public EnemyJumpState(HierarchicalStateMachine stateMachine, Enemy enemy, HSMState parent,
        IStateContainer factory) : base(stateMachine, parent)
    {
        movement = enemy.Movement;
        sensor = enemy.Sensor;  
        AllStates = factory;
    }

    protected override HSMState GetInitialState() => null;

    protected override HSMState GetTransition()
    {
        if (movement.IsFalling)
        {
            return AllStates.Fall;
        } //at apex 

        return null;
    }

    protected override void OnEnter()
    {
        //movement.Jump();
    }

    protected override void OnPhysicsTick(float fixedDeltaTime)
    {
        if (sensor.Target != null)
        {
            movement.Move(sensor.Target.position);
        }
    }
}
