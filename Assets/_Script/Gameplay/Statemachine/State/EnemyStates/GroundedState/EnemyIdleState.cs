using UnityEngine;

public class EnemyIdleState : State
{
    private EnemyMovement movement;
    private EnemySensor sensor;
    private EnemyAttack attack;
    
    public EnemyIdleState(HierarchicalStateMachine stateMachine, Enemy character, State parent, 
        ICharacterStateFactory factory) : base(stateMachine, character, parent, factory)
    {
        movement = character.Movement;
        sensor = character.Sensor;
        attack = character.AttackSystem;
    }

    protected override State GetInitialState() => null;

    protected override State GetTransition()
    {
        bool shouldMove = sensor.HasTarget && sensor.DistanceToTarget > attack.AttackRange;
        if (shouldMove)
        {
            return Factory.Run;
        }

        return null;
    }

    protected override void OnEnter()
    {
        Debug.Log("enemy idle enter");
        movement.StopMove();
    }

    protected override void OnTick(float deltaTime)
    {
        //Debug.Log("enemy idling");
    }
}
