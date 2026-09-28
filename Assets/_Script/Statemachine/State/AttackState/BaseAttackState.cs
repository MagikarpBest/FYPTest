using UnityEngine;

public class BaseAttackState : State
{
    private Player player;
    public BaseAttackState(HierarchicalStateMachine stateMachine, ICharacter characterContext, State parent,
        PlayerStateFactory factory) : base(stateMachine, characterContext, parent, factory)
    {
        if(characterContext is Player player)
        {
            this.player = player;
        }
    }
    
    
    protected override void OnEnter()
    {
        player.SkillController.SetSkillUsable(false);
    }

    protected override void OnExit()
    {
        player.SkillController.SetSkillUsable(true);
    }

}
