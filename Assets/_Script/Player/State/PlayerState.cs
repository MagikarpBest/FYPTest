
public abstract class PlayerState
{
    protected CompositeState parent;
    protected PlayerStateMachine stateMachine;
    protected PlayerInputManager Input => stateMachine.Input;
    protected PlayerMovementRB Movement => stateMachine.Movement;
    
    protected PlayerState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }
    
    public virtual void Enter() { }
    
    public virtual void Exit() { }
    
    public virtual void Update() { }
    
    public virtual void FixedUpdate() { }
    
    // Input handlers
    public virtual void HandleJump() {}
    public virtual void HandleAttack() {}
}
