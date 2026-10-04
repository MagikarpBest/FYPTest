using UnityEngine;

public class EnemyRunState : State
{
    private EnemyMovement movement;
    private EnemySensor sensor;
    
    public EnemyRunState(HierarchicalStateMachine stateMachine, Enemy enemy, State parent,
        ICharacterStateFactory factory)
        : base(stateMachine, enemy, parent, factory)
    {
        movement = enemy.Movement;
    }

    protected override State GetInitialState() => null;

    protected override State GetTransition()
    {
        if (!movement.IsMoving)
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
