using UnityEngine;

public class EnemyAirborneState : HSMState
{
    private EnemyMovement movement;
    IStateContainer AllStates;

    public EnemyAirborneState(HierarchicalStateMachine stateMachine, Enemy enemy, HSMState parent,
        IStateContainer factory) : base(stateMachine, parent)
    {
        movement = enemy.Movement;
        AllStates = factory;
    }

    protected override HSMState GetInitialState() => AllStates.Fall;

    private void HandleLaunch() => EventRequestTransition(AllStates.Launch);

    protected override HSMState GetTransition()
    {
        if (eventRequestedTransition != null)
        {
            return eventRequestedTransition;
        }

        return null;
    }

    // protected override void OnEnter()
    // {
    //     movement.OnLaunched += HandleLaunch;
    // }
    //
    // protected override void OnExit()
    // {
    //     movement.OnLaunched -= HandleLaunch;
    // }

    protected override void OnTick(float deltaTime)
    {

    }

    protected override void OnPhysicsTick(float fixedDeltaTime)
    {

    }
}
