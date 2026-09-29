using UnityEngine;

public class BaseAttackState : State
{
    private Player player;

    public BaseAttackState(HierarchicalStateMachine stateMachine, ICharacter characterContext, State parent,
        PlayerStateFactory factory) : base(stateMachine, characterContext, parent, factory)
    {
        if (characterContext is Player player)
        {
            this.player = player;
        }
    }

    protected override State GetTransition()
    {
        if (!player.AttackSystem.isAttacking)
        {
            return Factory.Grounded;
        }
        return null;
    }

    protected override void OnEnter()
    {
        player.SkillController.SetSkillUsable(false);
        player.AttackSystem.HandleAtack();
        player.Movement.StopMove(); // bandaid fix

    }

    protected override void OnExit()
    {
        player.SkillController.SetSkillUsable(true);
    }

}
