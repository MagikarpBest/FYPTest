using UnityEngine;

public abstract class State
{
    protected PlayerStateFactory Factory;
    private readonly HierarchicalStateMachine StateMachine;
    public readonly State Parent;
    public State ActiveChild;
    protected readonly ICharacter CharacterContext;

    public State(HierarchicalStateMachine stateMachine, ICharacter characterContext, State parent,
        PlayerStateFactory factory)
    {
        StateMachine = stateMachine;
        Factory = factory;
        Parent = parent;
        CharacterContext = characterContext;
    }

    protected virtual State GetInitialState() =>
        null; //initial child to enter when this state starts (null = this is the leaf)

    protected virtual State GetTransition() =>
        null; //target state to switch to this frame (null = stay in current state)

    protected State eventRequestedTransition;
    protected void EventRequestTransition(State state) => eventRequestedTransition = state;


    public void Enter()
    {
        //set parent 
        if (Parent != null) Parent.ActiveChild = this;

        //run enter logic
        OnEnter();

        //check for initial child state and enter
        State initialState = GetInitialState();
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
            //ActiveChild.Exit();
            ActiveChild = null;
        }

        eventRequestedTransition = null;

        //run exit logic
        OnExit();
    }

    public void Tick(float deltaTime)
    {
        //check for transitions
        State transitionState = GetTransition();
        if (transitionState != null)
        {
            eventRequestedTransition = null;
            StateMachine?.ChangeState(this, transitionState);
            return;
        }

        //tick child
        if (ActiveChild != null)
        {
            ActiveChild.Tick(deltaTime);
        }

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


    //for transition logic later like example you want a delay before entering the state
    //protected virtual State GetTransition(float value)
    //{
    //    foreach (TransitionPair transition in transitions)
    //    {
    //        if (transition.predicate(value))
    //            return transition.nextState;
    //    }

    //    return null;
    //}
}
