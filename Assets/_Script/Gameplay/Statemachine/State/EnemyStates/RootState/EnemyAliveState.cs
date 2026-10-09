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
        if (eventRequestedTransition != null)
        {
            return eventRequestedTransition;
        }

        return null;
    }

    private void HandleReaction(EnemyReactionType reactionType)
    {
        switch (reactionType)
        {
            case EnemyReactionType.Hit:
                EventRequestTransition(Factory.Hit);
                break;

            case EnemyReactionType.Launch:
                EventRequestTransition(Factory.Launch);
                break;

            case EnemyReactionType.Dead:
                EventRequestTransition(Factory.Dead);
                break;

            case EnemyReactionType.None:
                EventRequestTransition(Factory.Grounded);
                break;
        }
    }

    protected override void OnEnter()
    {
        character.DisableRagdoll();
        combatReaction.OnReactionRequested += HandleReaction;
    }

    protected override void OnExit()
    {
        combatReaction.OnReactionRequested -= HandleReaction;
    }
}
