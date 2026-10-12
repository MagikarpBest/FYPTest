using UnityEngine;

public class JumpState : HSMState
{
    private PlayerMovementRB movement;
    private PlayerInputManager input;
    private Player player;
    private IStateContainer AllStates;

    public JumpState(HierarchicalStateMachine stateMachine, Player player, HSMState parent,
        IStateContainer factory) : base(stateMachine, parent)
    {
        this.player = player;
        movement = player.Movement;
        input = player.Input;
        
        
        AllStates = factory;
    }

    protected override HSMState GetInitialState() => null;

    protected override HSMState GetTransition()
    {
        if (player.Rigidbody.linearVelocity.y <= 0f)
        {
            return AllStates.Fall;
        } //at apex 

        return null;
    }

    protected override void OnEnter()
    {
        movement.Jump();
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
