using UnityEngine;

public class LaunchState : State
{
    private PlayerMovementRB movement;
    private PlayerInputManager input;

    public LaunchState(HierarchicalStateMachine stateMachine, ICharacter character, State parent,
        ICharacterStateFactory factory) : base(stateMachine, character, parent, factory)
    {
        if (Character is Player player)
        {
            movement = player.Movement;
            input = player.Input;
        }
    }

    protected override State GetInitialState() => null;

    protected override State GetTransition()
    {
        if (!movement.IsLaunched)
        {
            return Factory.Fall;
        }
        return null;
    }

    protected override void OnEnter()
    {

    }


    protected override void OnTick(float deltaTime)
    {

    }
}
