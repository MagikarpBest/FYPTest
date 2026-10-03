public class AirborneState : State
{
    // rework
    private PlayerMovementRB movement;
    private PlayerInputManager input;

    public AirborneState(HierarchicalStateMachine stateMachine, ICharacter characterContext, State parent,
        ICharacterStateFactory factory) : base(stateMachine, characterContext, parent, factory)
    {

        if (CharacterContext is Player player)
        {
            movement = player.Movement;
            input = player.Input;
        }
    }

    protected override State GetInitialState() => Factory.Fall;

    private void HandleLaunch() => EventRequestTransition(Factory.Launch);

    protected override State GetTransition()
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
