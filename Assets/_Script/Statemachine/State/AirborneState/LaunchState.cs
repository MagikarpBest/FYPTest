using UnityEngine;

public class LaunchState : State
{
    private PlayerMovementRB movement;
    private PlayerInputManager input;

    public LaunchState(HierarchicalStateMachine stateMachine, ICharacter characterContext, State parent) : base(stateMachine, characterContext, parent)
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
        return null;
    }

    protected override void OnEnter()
    {
   
    }


    protected override void OnTick(float deltaTime)
    {
        
    }
}
