using System;
using System.Threading;
using UnityEngine;

public class Ability
{
    public readonly AbilityDefinitionSO Definition;
    private AbilitySystem ctx;
    private CancellationTokenSource cts;
    public bool IsRunning => cts != null;

    public Ability(AbilityDefinitionSO definition, AbilitySystem ctx)
    {
        Definition = definition; 
        this.ctx = ctx;
    }
    
    public bool CanActivateAbility() => 
        !IsRunning 
        && ctx.HasAllTags(Definition.ActivationRequiredTags)
        && !ctx.HasAnyTags(Definition.ActivationBlockedTags)
        && !ctx.AreAbilityTagsBlocked(Definition.AbilityTags);
    
    public async void Activate()
    {
        if (!CanActivateAbility()) return;

        cts = new CancellationTokenSource();
        bool cancelled = false;
        try
        {
            ctx.AddTags(Definition.GrantedTags);  
            ctx.NotifyAbilityActivated(this);
            await Definition.OnActivate(ctx, cts.Token); 
            Definition.OnEnd(ctx);
        }
        catch (OperationCanceledException)
        {
            cancelled = true;
            Definition.OnCancel(ctx);
        }
        finally
        {
            cts.Dispose();
            cts = null;
            ctx.RemoveTags(Definition.GrantedTags);
            
            if (cancelled) ctx.NotifyAbilityCancelled(this);
            else ctx.NotifyAbilityFinished(this);
        }
    }

    public void Cancel() => cts?.Cancel();
}
