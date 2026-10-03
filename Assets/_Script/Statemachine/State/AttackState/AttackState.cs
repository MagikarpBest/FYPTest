using UnityEngine;

public class AttackState : State
{
    private ICanUseSkills skills;
    private IHasInput input;
    private IHasAttack attacker;

    public AttackState(HierarchicalStateMachine stateMachine, ICharacter characterContext, State parent,
        ICharacterStateFactory factory) : base(stateMachine, characterContext, parent, factory)
    {
        skills = characterContext as ICanUseSkills;
        input = characterContext as IHasInput;
        attacker = characterContext as IHasAttack;
    }

    protected override State GetInitialState() => Factory.BaseAttack;

    protected override void OnEnter()
    {
        skills?.SetSkillUsable(false);
        if (input != null)
        {
            input.OnAttackPressed += HandleAttack;
        }
    }

    protected override void OnExit()
    {
        skills?.SetSkillUsable(true);
        if (input != null)
        {
            input.OnAttackPressed -= HandleAttack;
        }
    }
    private void HandleAttack()
    {
        attacker?.HandleAtack();
    }

}
