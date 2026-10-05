using UnityEngine;

public class EnemyDeadState : State
{
    private EnemyMovement movement;
    private EnemySensor sensor;
    
    public EnemyDeadState(HierarchicalStateMachine stateMachine, Enemy character, State parent,
        ICharacterStateFactory factory) : base(stateMachine, character, parent, factory)
    {
        movement = character.Movement;
        sensor = character.Sensor;  
    }
    
    protected override void OnEnter()
    {
        //dead
        sensor.enabled = false;
        movement.StopMove();
    }
}
