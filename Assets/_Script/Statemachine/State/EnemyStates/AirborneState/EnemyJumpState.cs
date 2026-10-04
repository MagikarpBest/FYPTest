using UnityEngine;

public class EnemyJumpState : State
{
    private EnemyMovement movement;
    private EnemySensor sensor;

    public EnemyJumpState(HierarchicalStateMachine stateMachine, Enemy character, State parent,
        ICharacterStateFactory factory) : base(stateMachine, character, parent, factory)
    {
        movement = character.Movement;
        sensor = character.Sensor;  
    }

    protected override State GetInitialState() => null;

    protected override State GetTransition()
    {
        if (movement.IsFalling)
        {
            return Factory.Fall;
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
