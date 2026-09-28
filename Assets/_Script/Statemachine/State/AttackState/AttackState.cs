using UnityEngine;

public class AttackState : State
{
    private Player player;

    public AttackState(HierarchicalStateMachine stateMachine, ICharacter characterContext, State parent,
        PlayerStateFactory factory) : base(stateMachine, characterContext, parent, factory)
    {
        if(characterContext is Player player)
        {
            this.player = player;
        }
    }

    protected override State GetInitialState() => Factory.BaseAttack;

    protected override void OnEnter()
    {
        player.SkillController.SetSkillUsable(false);
    }

    protected override void OnExit()
    {
        player.SkillController.SetSkillUsable(true);
    }

}
