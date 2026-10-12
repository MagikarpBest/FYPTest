using UnityEngine;

public class LaunchState : HSMState
{
    private PlayerMovementRB movement;
    private PlayerInputManager input;
    private IStateContainer AllStates;

    public LaunchState(HierarchicalStateMachine stateMachine, Player player, HSMState parent,
        IStateContainer factory) : base(stateMachine, parent)
    { 
        movement = player.Movement; 
        input = player.Input;
        
        AllStates = factory;
    }

    protected override HSMState GetInitialState() => null;

    protected override HSMState GetTransition()
    {
        if (!movement.IsLaunched)
        {
            return AllStates.Fall;
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
