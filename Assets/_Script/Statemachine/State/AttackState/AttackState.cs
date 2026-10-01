using UnityEngine;

public class AttackState : State
{
    private ICanUseSkills skills;

    public AttackState(HierarchicalStateMachine stateMachine, ICharacter characterContext, State parent,
        ICharacterStateFactory factory) : base(stateMachine, characterContext, parent, factory)
    {
        skills = characterContext as ICanUseSkills;
    }

    protected override State GetInitialState() => Factory.BaseAttack;

    protected override void OnEnter()
    {
        skills.SetSkillUsable(false);
    }

    protected override void OnExit()
    {
        skills.SetSkillUsable(true);
    }

}
