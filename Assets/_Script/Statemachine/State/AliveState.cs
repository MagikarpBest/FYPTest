using UnityEngine;

public class AliveState : State
{
    public readonly GroundedState Grounded;
    public readonly AirborneState Airborne;

    public AliveState(HierarchicalStateMachine stateMachine, ICharacter characterContext, State parent) : base(stateMachine, characterContext, parent)
    {
        Grounded = new GroundedState(stateMachine, characterContext, this);
        Airborne = new AirborneState(stateMachine, characterContext, this);
    }

    protected override State GetInitialState() => Grounded;
    
}
