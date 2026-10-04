using UnityEngine;

public class RunState : State
{
    private PlayerMovementRB movement;
    private PlayerInputManager input;
    
    public RunState(HierarchicalStateMachine stateMachine, Player player, State parent,
        ICharacterStateFactory factory)
        : base(stateMachine, player, parent, factory)
    {
        movement = player.Movement;
        input = player.Input;
    }

    protected override State GetInitialState() => null;

    protected override State GetTransition()
    {
        if (input.MoveInput == Vector2.zero)
        {
            return Factory.Idle;
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
