using UnityEngine;

public class FallState : State
{
    private PlayerMovementRB movement;
    private PlayerInputManager input;

    public FallState(HierarchicalStateMachine stateMachine, ICharacter characterContext, State parent) : base(stateMachine, characterContext, parent)
    {
        if (CharacterContext is Player player)
        {
            movement = player.Movement;
            input = player.Input;
        }
    }
    
    protected override State GetInitialState() => null;
    
    protected override State GetTransition()
    {
        if (!movement.IsLaunched && movement.IsGrounded) return ((AliveState)Parent.Parent).Grounded;
        
        return null;
    }
    
    protected override void OnPhysicsTick(float fixedDeltaTime)
    {
        movement.Move(input.MoveInput);
        movement.RotateTowardsMovement();
    }

}
