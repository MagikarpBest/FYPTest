using UnityEngine;

public class IdleState : HSMState
{
    private PlayerMovementRB movement;
    private PlayerInputManager input;
    private IStateContainer AllStates;

    public IdleState(HierarchicalStateMachine stateMachine, Player player, HSMState parent,
        IStateContainer factory) : base(stateMachine, parent)
    {
        movement = player.Movement;
        input = player.Input;
        AllStates = factory;
    }

    protected override HSMState GetInitialState() => null;

    protected override HSMState GetTransition()
    {
        if (input.MoveInput != Vector2.zero)
        {
            return AllStates.Run;
        }

        return null;
    }

    protected override void OnEnter()
    {
        movement.StopMove();
    }
}
