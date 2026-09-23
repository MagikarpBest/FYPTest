

public class AirborneState : CompositeState
{
    public AirborneState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        ChangeChild(stateMachine.Fall);
    }

    public override void FixedUpdate()
    {
        Movement.Move(Input.MoveInput);
        Movement.RotateTowardsMovement();
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
