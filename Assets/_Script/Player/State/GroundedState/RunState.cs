using UnityEngine;

public class RunState : PlayerState
{
    public RunState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }
    
    public override void FixedUpdate()
    {
        Movement.Move(Input.MoveInput);
        Movement.RotateTowardsMovement();
    }
    
    public override void Update()
    {
        if (Input.MoveInput == Vector2.zero)
        {
            stateMachine.Grounded.ChangeChild(stateMachine.Idle);
        }
    }
}
