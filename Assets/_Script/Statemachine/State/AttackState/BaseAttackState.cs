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
        if (attacker != null && !attacker.isAttacking )
        {
            if(attacker.isComboQueued)
            {
                Debug.Log($"{attacker.isAttacking}, {attacker.isComboQueued}");
                return Factory.BaseAttack;
            }
            return Factory.Grounded;
        }
        return null;
    }

    protected override void OnEnter()
    {
        Debug.Log("On base attackstate enter ");
        skills?.SetSkillUsable(false);
        mover?.StopMove();
        
        // If we are entering this state and we have a queue, it means we are continuing a combo
        if(attacker.isComboQueued||!attacker.isAttacking)
        {
           //Debug.Log("On base attackstate enter and attack");
            attacker?.HandleAtack();
        }
    }

    protected override void OnExit()
    {
        //Debug.Log("On base attackstate exit ");
        skills?.SetSkillUsable(true);
    }
}
