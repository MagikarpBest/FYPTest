using UnityEngine;

public class FallState : PlayerState
{
    private AirborneState parent;

    public FallState(PlayerStateMachine stateMachine, AirborneState parent) : base(stateMachine)
    {
        this.parent = parent;
    }

    public override void FixedUpdate()
    {
        Movement.Move(Input.MoveInput);
        Movement.RotateTowardsMovement();
    }

}
