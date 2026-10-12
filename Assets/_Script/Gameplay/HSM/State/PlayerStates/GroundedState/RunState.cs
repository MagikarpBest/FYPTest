using UnityEngine;

public class RunState : HSMState
{
    private PlayerMovementRB movement;
    private PlayerInputManager input;
    private IStateContainer AllStates;

    public RunState(HierarchicalStateMachine stateMachine, Player player, HSMState parent,
        IStateContainer factory)
        : base(stateMachine, parent)
    {
        movement = player.Movement;
        input = player.Input;
        AllStates = factory;
    }

    protected override HSMState GetInitialState() => null;

    protected override HSMState GetTransition()
    {
        if (input.MoveInput == Vector2.zero)
        {
            return AllStates.Idle;
        }

        return null;
    }

    protected override void OnEnter()
    {
    }

    protected override void OnExit()
    {
        //movement.StopMove();
    }
    
    protected override void OnTick(float deltaTime)
    {
        movement.RotateTowardsMovement();
    }
    
    protected override void OnPhysicsTick(float fixedDeltaTime)
    {
        movement.Move(input.MoveInput);
    }
}
