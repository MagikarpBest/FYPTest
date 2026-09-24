using UnityEngine;

public class RunState : PlayerState
{
    private GroundedState parent;

    public RunState(PlayerStateMachine stateMachine,GroundedState parent) : base(stateMachine)
    {
        this.parent = parent;
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
            parent.ChangeChild(parent.Idle);
        }
    }
}
