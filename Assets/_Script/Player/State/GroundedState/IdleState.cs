using UnityEngine;

public class IdleState : PlayerState
{
    private GroundedState parent;

    public IdleState(PlayerStateMachine stateMachine,GroundedState parent) : base(stateMachine)
    {
        this.parent = parent;
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
            parent.ChangeChild(parent.Run);
        }
    }
}
