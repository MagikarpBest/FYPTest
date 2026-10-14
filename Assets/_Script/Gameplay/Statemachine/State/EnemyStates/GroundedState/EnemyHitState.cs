using UnityEngine;

public class EnemyHitState : State
{
    private EnemyMovement movement;
    private EnemyCombatReaction reaction;
    public EnemyHitState(HierarchicalStateMachine stateMachine, Enemy character, State parent,
        ICharacterStateFactory factory) : base(stateMachine, character, parent, factory)
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
