using UnityEngine;

public class BaseAttackState : State
{
    private PlayerAttack attackSystem;
    private PlayerMovementRB movement;
    private PlayerSkillController skill;

    public BaseAttackState(HierarchicalStateMachine stateMachine, Player player, State parent,
        ICharacterStateFactory factory) : base(stateMachine, player, parent, factory)
    {
        attackSystem = player.AttackSystem;
        movement = player.Movement;
        skill = player.SkillController;
    }

    protected override State GetTransition()
    {
        if (!attackSystem.IsAttacking)
        {
            if (attackSystem.IsComboQueued)
            {
                //Debug.Log($"{attacker.isAttacking}, {attacker.isComboQueued}");
                return Factory.BaseAttack;
            }
            return Factory.Grounded;
        }
        
        return null;
    }

    protected override void OnEnter()
    {
        Debug.Log("On base attackstate enter ");
        skill.SetSkillUsable(false);
        movement.StopMove();
        
        // If we are entering this state and we have a queue, it means we are continuing a combo
        if(attackSystem.IsComboQueued || !attackSystem.IsAttacking)
        {
           //Debug.Log("On base attackstate enter and attack");
            attackSystem.RequestAttack();
        }
    }

    protected override void OnExit()
    {
        //Debug.Log("On base attackstate exit ");
        skill.SetSkillUsable(true);
    }
}
