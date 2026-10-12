using UnityEngine;

public class EnemyFallState : HSMState
{
    private EnemyMovement movement;
    private EnemySensor sensor;
    private IStateContainer AllStates;
    public EnemyFallState(HierarchicalStateMachine stateMachine, Enemy enemy, HSMState parent,
        IStateContainer factory) : base(stateMachine, parent)
    {
        movement = enemy.Movement;
        sensor = enemy.Sensor;  
        AllStates = factory;
    }
    
    protected override HSMState GetInitialState() => null;
    
    protected override HSMState GetTransition()
    {
        if (!movement.IsLaunched && movement.IsGrounded)
        {
            return AllStates.Grounded;
        }
        
        return null;
    }
    
    // protected override void OnTick(float deltaTime)
    // {
    //     movement.RotateTowardsMovement();
    // }
    //
    protected override void OnPhysicsTick(float fixedDeltaTime)
    {
        if (sensor.Target != null)
        {
            movement.Move(sensor.Target.position);
        }
    }
}
