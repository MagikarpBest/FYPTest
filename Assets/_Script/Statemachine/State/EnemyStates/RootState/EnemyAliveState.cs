using UnityEngine;

public class EnemyAliveState : State
{
    public EnemyAliveState(HierarchicalStateMachine stateMachine, Enemy enemy, State parent,
        ICharacterStateFactory factory) : base(stateMachine, enemy, parent, factory)
    {

    }

    protected override State GetInitialState() => Factory.Grounded;

    protected override State GetTransition()
    {
        // TODO dead
        // if (CharacterContext.IsDead)
        // {
        //     return Factory.Dead;
        // }
        return null;
    }
}
