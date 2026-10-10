using System;
using System.Threading;
using UnityEngine;

public abstract class AbilityDefinitionSO : ScriptableObject
{
    [SerializeField] private GameplayTagContainer abilityTags; //tags used to identity this ability
    [SerializeField] private GameplayTagContainer activationRequiredTags; 
    [SerializeField] private GameplayTagContainer activationBlockedTags;
    [SerializeField] private GameplayTagContainer cancelAbilitiesWithTag; //cancel other abilities with these tags when this ability activates
    [SerializeField] private GameplayTagContainer blockAbilitiesWithTag; //prevent activation of other abilities with these tags when this ability activates
    [SerializeField] private GameplayTagContainer grantedTags; //give these tags when this ability is running
    
    public GameplayTagContainer AbilityTags => abilityTags;
    public GameplayTagContainer ActivationRequiredTags => activationRequiredTags;
    public GameplayTagContainer ActivationBlockedTags => activationBlockedTags; 
    public GameplayTagContainer CancelAbilitiesWithTag => cancelAbilitiesWithTag;
    public GameplayTagContainer BlockAbilitiesWithTag => blockAbilitiesWithTag;
    public GameplayTagContainer GrantedTags => grantedTags;

    public virtual Awaitable OnActivate(AbilitySystem ctx, CancellationToken token) => Awaitable.NextFrameAsync(token);
    public virtual void OnEnd(AbilitySystem ctx) {}
    public virtual void OnCancel(AbilitySystem ctx) {}

}
