public class DeadState : HSMState
{
    private PlayerMovementRB movement;
    public DeadState(HierarchicalStateMachine stateMachine, Player player, HSMState parent,
        IStateContainer factory) : base(stateMachine, parent)
    {
        movement = player.Movement;
        
    }
    
    protected override void OnEnter()
    {
        //dead
        movement.StopMove();
    }
}
