using UnityEngine;

public class EnemyDeadState : State
{
    private EnemyMovement movement;
    private EnemySensor sensor;
    
    public EnemyDeadState(HierarchicalStateMachine stateMachine, Enemy enemy, State parent,
        ICharacterStateFactory factory) : base(stateMachine, enemy, parent, factory)
    {
        movement = enemy.Movement;
        sensor = enemy.Sensor;  
    }
    
    protected override void OnEnter()
    {
        //dead
        sensor.enabled = false;
        movement.StopMove();
    }
}
