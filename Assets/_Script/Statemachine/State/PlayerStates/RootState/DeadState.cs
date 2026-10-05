public class DeadState : State
{
    private PlayerMovementRB movement;
    public DeadState(HierarchicalStateMachine stateMachine, Player player, State parent,
        ICharacterStateFactory factory) : base(stateMachine, player, parent, factory)
    {
        movement = player.Movement;
    }
    
    protected override void OnEnter()
    {
        //dead
        movement.StopMove();
    }
}
