using UnityEngine;

public class FallState : HSMState
{
    private PlayerMovementRB movement;
    private PlayerInputManager input;
    private IStateContainer AllStates;

    public FallState(HierarchicalStateMachine stateMachine, Player player, HSMState parent,
        IStateContainer factory) : base(stateMachine,parent)
    { 
        movement = player.Movement; 
        input = player.Input;
        
        AllStates = factory;
    }
    
    protected override HSMState GetInitialState() => null;
    
    protected override HSMState GetTransition()
    {
        if (!movement.IsLaunched && movement.IsGrounded)
        {
            return AllStates.Grounded;
        }
        
        return null;
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
