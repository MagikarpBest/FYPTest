using UnityEngine;

public class AliveState : State
{
    private PlayerStatus status;
    
    public AliveState(HierarchicalStateMachine stateMachine, Player player, State parent,
        ICharacterStateFactory factory) : base(stateMachine, player, parent, factory)
    {
        status = player.Status;
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
