using System;
using System.Threading;
using UnityEngine;

[Serializable]
public class Ability
{
    public readonly AbilityDefinitionSO Definition;
    private AbilitySystem ctx;
    private CancellationTokenSource cts;
    public bool IsActive => cts != null;

    public Ability(AbilityDefinitionSO definition, AbilitySystem ctx)
    {
        Definition = UnityEngine.Object.Instantiate(definition); 
        Definition.Init(ctx);
        this.ctx = ctx;
    }
    
    public bool CanActivateAbility() => 
        !IsActive 
        && ctx.HasAllTags(Definition.ActivationRequiredTags)
        && !ctx.HasAnyTags(Definition.ActivationBlockedTags)
        && !ctx.AreAbilityTagsBlocked(Definition.AbilityTags);
    
    public async void Activate()
    {
        //if (!CanActivateAbility()) return;
        ctx.AddTag("State.Performing.Ability");
        cts = new CancellationTokenSource();
        bool cancelled = false;
        try
        {
            ctx.AddTags(Definition.GrantedTags);  
            ctx.NotifyAbilityActivated(this);
            await Definition.OnActivate(cts.Token); 
            Definition.OnEnd();
        }
        catch (OperationCanceledException)
        {
            cancelled = true;
            Definition.OnCancel();
        }
        finally
        {
            cts.Dispose();
            cts = null;
            ctx.RemoveTag("State.Performing.Ability");
            ctx.RemoveTags(Definition.GrantedTags);
            Definition.Cleanup();
            if (cancelled) ctx.NotifyAbilityCancelled(this);
            else ctx.NotifyAbilityFinished(this);
        }
    }

    public void Cancel() => cts?.Cancel();
}
