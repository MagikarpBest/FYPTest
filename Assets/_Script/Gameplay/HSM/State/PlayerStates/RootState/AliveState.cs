using UnityEngine;

public class AliveState : HSMState
{
    private PlayerStats _stats;
    private IStateContainer AllStates;
    
    public AliveState(HierarchicalStateMachine stateMachine, Player player, HSMState parent,
        IStateContainer factory) : base(stateMachine, parent)
    {
        AllStates = factory;
        _stats = player.Stats;
    }

    protected override HSMState GetInitialState() => AllStates.Grounded;

    protected override HSMState GetTransition()
    {
        // if (status.CurrentHealth <= 0)
        // {
        //     return Factory.Dead;
        // }
        return null;
    }

}
