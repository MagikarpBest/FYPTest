using UnityEngine;

public class EnemyFallState : State
{
    private EnemyMovement movement;
    private EnemySensor sensor;

    public EnemyFallState(HierarchicalStateMachine stateMachine, Enemy enemy, State parent,
        ICharacterStateFactory factory) : base(stateMachine, enemy, parent, factory)
    {
        movement = enemy.Movement;
        sensor = enemy.Sensor;  
    }
    
    protected override State GetInitialState() => null;
    
    protected override State GetTransition()
    {
        if (!movement.IsLaunched && movement.IsGrounded)
        {
            return Factory.Grounded;
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
