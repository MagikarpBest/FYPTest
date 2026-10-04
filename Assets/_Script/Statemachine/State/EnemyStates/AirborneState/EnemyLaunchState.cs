using UnityEngine;

public class EnemyLaunchState : State
{
    private EnemyMovement movement;

    public EnemyLaunchState(HierarchicalStateMachine stateMachine, Enemy enemy, State parent,
        ICharacterStateFactory factory) : base(stateMachine, enemy, parent, factory)
    { 
        movement = enemy.Movement; 
    }

    protected override State GetInitialState() => null;

    protected override State GetTransition()
    {
        if (!movement.IsLaunched)
        {
            return Factory.Fall;
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
