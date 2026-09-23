
public class GroundedState : CompositeState
{
    public GroundedState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        
    }
    
    public override void Enter()
    {
        ChangeChild(new Idle);
    }
}
