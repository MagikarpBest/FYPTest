
public abstract class CompositeState : PlayerState
{
    protected PlayerState currentChild;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected CompositeState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        
    }

    public void ChangeChild(PlayerState child)
    {
        currentChild?.Exit();

        currentChild = child;
        
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
