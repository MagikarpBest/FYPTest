using UnityEngine;

public class IdleState : PlayerState
{
    public IdleState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        Movement.StopMove();
    }

    public override void Update()
    {
        // if wasd is pressed change to run state
        if (Input.MoveInput != Vector2.zero)
        {
            stateMachine.Grounded.ChangeChild(stateMachine.Run);
        }
    }
}
