using UnityEngine;

public class EnemyLaunchState : HSMState
{
    private EnemyMovement movement;
    private IStateContainer AllStates;
    public EnemyLaunchState(HierarchicalStateMachine stateMachine, Enemy enemy, HSMState parent,
        IStateContainer factory) : base(stateMachine, parent)
    { 
        movement = enemy.Movement;
        AllStates = factory;
    }

    protected override HSMState GetInitialState() => null;

    protected override HSMState GetTransition()
    {
        if (!movement.IsLaunched)
        {
            return AllStates.Fall;
        }
        return null;
    }

    protected override void OnEnter()
    {

    }


    protected override void OnTick(float deltaTime)
    {

    }
}
