using UnityEngine;

public class LaunchState : CompositeState
{

    public LaunchState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Update()
    {
        if(!Movement.IsLaunched&&Movement.IsGrounded)
        {
            stateMachine.Grounded.ChangeChild(stateMachine.Idle);
        }
        base.Update();
    }
}
