public class AirborneState : HSMState
{
    private PlayerMovementRB movement;
    private PlayerInputManager input;
    private IStateContainer AllStates;

    public AirborneState(HierarchicalStateMachine stateMachine, Player player, HSMState parent,
        IStateContainer factory) : base(stateMachine, parent)
    {
            movement = player.Movement;
            input = player.Input;
            AllStates = factory;
    }

    protected override HSMState GetInitialState() => AllStates.Fall;

    private void HandleLaunch() => EventRequestTransition(AllStates.Launch);

    protected override HSMState GetTransition()
    {
        if (eventRequestedTransition != null)
        {
            return eventRequestedTransition;
        }

        return null;
    }

    protected override void OnEnter()
    {
        movement.OnLaunched += HandleLaunch;
    }
    
    protected override void OnExit()
    {
        movement.OnLaunched -= HandleLaunch;
    }

    protected override void OnTick(float deltaTime)
    {

    }

    protected override void OnPhysicsTick(float fixedDeltaTime)
    {

    }
}
