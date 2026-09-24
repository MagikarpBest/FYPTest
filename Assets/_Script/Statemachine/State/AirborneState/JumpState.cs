using UnityEngine;

public class JumpState : State
{
    private PlayerMovementRB movement;
    private PlayerInputManager input;
    private Rigidbody rb;
    
    public JumpState(HierarchicalStateMachine stateMachine, ICharacter characterContext, State parent) : base(stateMachine, characterContext, parent)
    {
        if (CharacterContext is Player player)
        {
            movement = player.Movement;
            input = player.Input;
            rb = player.Rigidbody;
        }
    }
    
    protected override State GetInitialState() => null;
    
    protected override State GetTransition()
    {
        if (rb.linearVelocity.y <= 0) return ((AirborneState)Parent).Fall; //at apex 

        return null;
    }

    protected override void OnEnter()
    {
        movement.Jump();
    }
    
    protected override void OnPhysicsTick(float fixedDeltaTime)
    {
        movement.Move(input.MoveInput);
        movement.RotateTowardsMovement();
    }
}
