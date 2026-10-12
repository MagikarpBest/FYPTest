using UnityEngine;

public abstract class HSMState
{
    private readonly HierarchicalStateMachine StateMachine;
    public readonly HSMState Parent;
    public HSMState ActiveChild;

    public HSMState(HierarchicalStateMachine stateMachine, HSMState parent)
    {
        StateMachine = stateMachine;
        Parent = parent;
    }

    protected virtual HSMState GetInitialState() => null; //initial child to enter when this state starts (null = this is the leaf)

    protected virtual HSMState GetTransition() => null; //target state to switch to this frame (null = stay in current state)

    protected HSMState eventRequestedTransition;
    protected void EventRequestTransition(HSMState state) => eventRequestedTransition = state;
    
    public void Enter()
    {
        //set parent 
        if (Parent != null) Parent.ActiveChild = this;
        
        //run enter logic
        OnEnter();

        //check for initial child state and enter
        HSMState initialState = GetInitialState();
        if (initialState != null)
        {
            initialState.Enter();
        }
    }

    public void Exit()
    {
        //clear child
        if (ActiveChild != null)
        {
            ActiveChild.Exit();
            ActiveChild = null;
        }
        
        //run exit logic
        OnExit();
    }

    public void Tick(float deltaTime)
    {
        //check for transitions
        HSMState transitionState = GetTransition();
        
        if (transitionState != null)
        {
            eventRequestedTransition = null;
            StateMachine.ChangeState(this, transitionState);
            return;
        }

        //tick child
        if (ActiveChild != null) ActiveChild.Tick(deltaTime);

        //run tick logic
        OnTick(deltaTime);
    }

    public void PhysicsTick(float fixedDeltaTime)
    {
        //tick child
        if (ActiveChild != null) ActiveChild.PhysicsTick(fixedDeltaTime);

        //run tick logic
        OnPhysicsTick(fixedDeltaTime);
    }

    //u do like this so u dont have to call base.whatever when u override
    protected virtual void OnEnter()
    {
    }

    protected virtual void OnExit()
    {
    }

    protected virtual void OnTick(float deltaTime)
    {
    }

    protected virtual void OnPhysicsTick(float fixedDeltaTime)
    {
    }
}
