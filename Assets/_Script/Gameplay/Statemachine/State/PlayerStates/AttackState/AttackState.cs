using UnityEngine;

public class AttackState : State
{
    private PlayerSkillController skill;
    private PlayerInputManager input;
    private PlayerAttack attackSystem;

    public AttackState(HierarchicalStateMachine stateMachine, Player player, State parent,
        ICharacterStateFactory factory) : base(stateMachine, player, parent, factory)
    {
        skill = player.SkillController;
        input = player.Input;
        attackSystem = player.AttackSystem;
    }

    protected override State GetInitialState() => Factory.BaseAttack;

    protected override void OnEnter()
    {
        skill.SetSkillUsable(false);
        //input.OnAttackPressed += HandleAttack;
    }

    protected override void OnExit()
    {
        skill.SetSkillUsable(true);
        //input.OnAttackPressed -= HandleAttack;
    }
    private void HandleAttack()
    {
        attackSystem.RequestAttack();
    }

}
