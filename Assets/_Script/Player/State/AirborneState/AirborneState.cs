

public class AirborneState : CompositeState
{
    public JumpState Jump { get; private set; }
    public FallState Fall { get; private set; }
    public LaunchState Launch { get; private set; }
    public AirborneState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        Jump = new JumpState(stateMachine, this);
        Fall = new FallState(stateMachine, this);
        Launch = new LaunchState(stateMachine, this);
    }
    
    public override void HandleLaunch()
    {
        ChangeChild(Launch);
    }

    public override void Enter()
    {
        ChangeChild(Fall);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Update()
    {
        if(Movement.IsGrounded)
        {
            stateMachine.ChangeState(stateMachine.Grounded);
            return;
        }
        base.Update();
    }
}
