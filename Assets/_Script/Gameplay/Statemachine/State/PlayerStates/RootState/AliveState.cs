using UnityEngine;

public class AliveState : State
{
    private PlayerStats _stats;
    
    public AliveState(HierarchicalStateMachine stateMachine, Player player, State parent,
        ICharacterStateFactory factory) : base(stateMachine, player, parent, factory)
    {
        _stats = player.Stats;
    }

    protected override State GetInitialState() => Factory.Grounded;

    protected override State GetTransition()
    {
        // if (status.CurrentHealth <= 0)
        // {
        //     return Factory.Dead;
        // }
        return null;
    }

}
