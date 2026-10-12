using UnityEngine;

public class EnemyHitState : HSMState
{
    private EnemyMovement movement;
    private EnemyCombatReaction reaction;
    public EnemyHitState(HierarchicalStateMachine stateMachine, Enemy character, HSMState parent,
        IStateContainer factory) : base(stateMachine, parent)
    {
        movement = character.Movement;
        reaction = character.CombatReaction;
    }

    protected override void OnEnter()
    {
        movement.StopMove();
        //test only
        reaction.HitAnimation();
        //reaction.StartCoroutine(reaction.ConsumeReaction());
    }
}
