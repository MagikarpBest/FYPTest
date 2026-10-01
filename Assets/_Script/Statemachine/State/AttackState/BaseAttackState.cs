using UnityEngine;

public class BaseAttackState : State
{
    private IHasAttack attacker;
    private IHasMovement mover;
    private ICanUseSkills skills;

    public BaseAttackState(HierarchicalStateMachine stateMachine, ICharacter characterContext, State parent,
        ICharacterStateFactory factory) : base(stateMachine, characterContext, parent, factory)
    {
        attacker = characterContext as IHasAttack;
        mover = characterContext as IHasMovement;
        skills = characterContext as ICanUseSkills;
    }

    protected override State GetTransition()
    {
        if (attacker != null && !attacker.isAttacking)
        {   
            return Factory.Grounded;
        }
        return null;
    }

    protected override void OnEnter()
    {
        skills?.SetSkillUsable(false);
        attacker?.HandleAtack();
        mover?.StopMove();
    }

    protected override void OnExit()
    {
        skills?.SetSkillUsable(true);
    }
}
