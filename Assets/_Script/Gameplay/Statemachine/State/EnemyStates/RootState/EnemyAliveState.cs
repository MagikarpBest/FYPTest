using UnityEngine;

public class EnemyAliveState : State
{
    private EnemyStats stats;
    private EnemyCombatReaction combatReaction;
    private Enemy character;
    
    public EnemyAliveState(HierarchicalStateMachine stateMachine, Enemy character, State parent,
        ICharacterStateFactory factory) : base(stateMachine, character, parent, factory)
    {
        stats = character.Stats;
        combatReaction = character.CombatReaction;
        this.character = character;
    }

    protected override State GetInitialState() => Factory.Grounded;

    protected override State GetTransition()
    {
        switch (combatReaction.PendingReaction)
        {
            case EnemyReactionType.Dead:
                return Factory.Dead;

            case EnemyReactionType.Launch:
                return Factory.Launch;

            case EnemyReactionType.Hit:
                return Factory.Hit;
        }
        return null;
    }
    
    protected override void OnEnter()
    {
        character.DisableRagdoll();
    }
}
