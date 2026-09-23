using UnityEngine;
public abstract class CompositeState : PlayerState
{
    protected PlayerState currentChild;
    
    protected CompositeState(PlayerStateMachine stateMachine) : base(stateMachine)
    {

    }

    
    public void ChangeChild(PlayerState newChild)
    {
        Debug.Log($"<color=red>Child change: {currentChild} -> {newChild}<color=red>");
        currentChild?.Exit();

        currentChild = newChild;
        
        currentChild?.Enter();
    }

    public override void Update()
    {
        currentChild?.Update();
    }
    
    public override void FixedUpdate()
    {
        currentChild?.FixedUpdate();
    }


    public override void Exit()
    {
        currentChild?.Exit();
    }
}
