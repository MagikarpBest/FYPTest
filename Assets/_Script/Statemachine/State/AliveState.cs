using UnityEngine;

public class AliveState : State
{
    public AliveState(HierarchicalStateMachine stateMachine, ICharacter characterContext, State parent,
        PlayerStateFactory factory) : base(stateMachine, characterContext, parent, factory)
    {

    }

    protected override State GetInitialState() => Factory.Grounded;

    protected override State GetTransition()
    {
        if (CharacterContext.IsDead)
        {
            return Factory.Dead;
        }
        return null;
    }

}
