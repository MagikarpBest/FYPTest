
public class GroundedState : CompositeState
{
    public IdleState Idle { get; private set; }
    public RunState Run { get; private set; }

    public GroundedState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        Idle = new IdleState(stateMachine, this);
        Run = new RunState(stateMachine, this);
    }
    
    public override void Enter()
    {
        ChangeChild(Idle);
    }
    

    public override void HandleJump()
    {
        // if future need do check do 
        // if (!combat.CanAct) return, etc etc
        stateMachine.ChangeState(stateMachine.Airborne);
        stateMachine.Airborne.ChangeChild(stateMachine.Airborne.Jump);
    }

    public override void Update()
    {
        if(!Movement.IsGrounded)
        {
            stateMachine.ChangeState(stateMachine.Airborne);
            return;
        }
        base.Update();
    }
    
}
