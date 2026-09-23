using UnityEngine;

public class LaunchState : CompositeState
{
    private AirborneState parent;

    public LaunchState(PlayerStateMachine stateMachine, AirborneState parent) : base(stateMachine)
    {
        this.parent = parent;
    }

    public override void Enter()
    {
        // animation etc dmg
        
    }

    public override void Update()
    {
        if (!Movement.IsLaunched && Movement.IsGrounded)
        {
            stateMachine.ChangeState(stateMachine.Grounded);
        }
        base.Update();
    }
}
