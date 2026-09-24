using UnityEngine;

public class JumpState : PlayerState
{
    private AirborneState parent;

    public JumpState(PlayerStateMachine stateMachine, AirborneState parent) : base(stateMachine)
    {
        this.parent = parent;
    }

    public override void Enter()
    {
        Movement.Jump();
    }

    public override void FixedUpdate()
    {
        Movement.Move(Input.MoveInput);
        Movement.RotateTowardsMovement();
    }
}
