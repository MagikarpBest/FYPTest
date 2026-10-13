using UnityEngine;

public class EnemyIdleState : HSMState
{
    private EnemyMovement movement;
    private EnemySensor sensor;
    private EnemyAttack attack;
    private IStateContainer AllStates;
    public EnemyIdleState(HierarchicalStateMachine stateMachine, Enemy character, HSMState parent, 
        IStateContainer factory) : base(stateMachine, parent)
    {
        movement = character.Movement;
        sensor = character.Sensor;
        attack = character.AttackSystem;
        AllStates = factory;
    }

    protected override HSMState GetInitialState() => null;

    protected override HSMState GetTransition()
    {
        bool shouldMove = sensor.HasTarget && sensor.DistanceToTarget > attack.AttackRange;
        if (shouldMove)
        {
            return AllStates.Run;
        }

        return null;
    }

    protected override void OnEnter()
    {
        //Debug.Log("enemy idle enter");
        movement.StopMove();
    }

    protected override void OnTick(float deltaTime)
    {
        //Debug.Log("enemy idling");
    }
}
