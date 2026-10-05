using UnityEngine;

public class EnemyAirborneState : State
{
    private EnemyMovement movement;

    public EnemyAirborneState(HierarchicalStateMachine stateMachine, Enemy enemy, State parent,
        ICharacterStateFactory factory) : base(stateMachine, enemy, parent, factory)
    {
        movement = enemy.Movement;
    }

    protected override State GetInitialState() => Factory.Fall;

    private void HandleLaunch() => EventRequestTransition(Factory.Launch);

    protected override State GetTransition()
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
