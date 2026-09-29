
public class DeadState : State
{
    
    public DeadState(HierarchicalStateMachine stateMachine, ICharacter characterContext, State parent, PlayerStateFactory factory) : base(stateMachine, characterContext, parent, factory)
    {
    }

    protected override void OnEnter()
    {
        if (CharacterContext is Player player)
        {
            player.Movement.StopMove();
            // die
        }
    }
}
