using UnityEngine;

public class IdleState : PlayerState
{
    public IdleState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        // idle animation maybe
    }

    public override void Update()
    {
        // if wasd is pressed change to run state
        if(Input.MoveInput!=Vector2.zero)
        {
            stateMachine.ChangeState(new RunState(stateMachine));
            return;
        }
        
        if(!Movement.IsGrounded)
        {
            stateMachine.ChangeState(new FallState(stateMachine));
            return;
        }
    }
}
