using UnityEngine;

public class EnemyAliveState : HSMState
{
    private EnemyStats stats;
    private EnemyCombatReaction combatReaction;
    private Enemy character;
    private IStateContainer AllStates;
    public EnemyAliveState(HierarchicalStateMachine stateMachine, Enemy character, HSMState parent,
        IStateContainer factory) : base(stateMachine, parent)
    {
        stats = character.Stats;
        combatReaction = character.CombatReaction;
        this.character = character;
        AllStates = factory;
    }

    protected override HSMState GetInitialState() => AllStates.Grounded;

    protected override HSMState GetTransition()
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
                EventRequestTransition(AllStates.Hit);
                break;

            case EnemyReactionType.Launch:
                EventRequestTransition(AllStates.Launch);
                break;

            case EnemyReactionType.Dead:
                EventRequestTransition(AllStates.Dead);
                break;

            case EnemyReactionType.None:
                EventRequestTransition(AllStates.Grounded);
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
