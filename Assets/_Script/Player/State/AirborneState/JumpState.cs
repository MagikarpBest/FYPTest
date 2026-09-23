using UnityEngine;

public class JumpState : PlayerState
{

    public JumpState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        Movement.HandleJump();
    }
}
