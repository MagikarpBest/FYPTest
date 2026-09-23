
public class GroundedState : CompositeState
{
    public GroundedState(PlayerStateMachine stateMachine) : base(stateMachine)
    {

    }
    
    public override void Enter()
    {
        ChangeChild(stateMachine.Idle);
    }
    

    public override void HandleJump()
    {
        // if future need do check do 
        // if (!combat.CanAct) return, etc etc
        stateMachine.ChangeState(stateMachine.Airborne);
        stateMachine.Airborne.ChangeChild(stateMachine.Jump);
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
