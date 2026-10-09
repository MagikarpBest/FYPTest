using System;
using System.Threading;
using UnityEngine;

public class Ability : MonoBehaviour
{
    readonly AbilityDefinitionSO def;
    private AbilitySystem ctx;
    private CancellationTokenSource cts;
    
    public bool IsRunning => cts != null;

    public Ability(AbilityDefinitionSO def, AbilitySystem ctx)
    {
        this.def = def; 
        this.ctx = ctx;
    }
    
    public bool CanActivateAbility() => 
        !IsRunning 
        && ctx.Tags.HasAll(def.ActivationRequiredTags)
        && !ctx.Tags.HasAny(def.ActivationBlockedTags);
    
    public async void Activate()
    {
        if (!CanActivateAbility()) return;

        cts = new CancellationTokenSource();
        try
        {
            await def.OnActivate(ctx, cts.Token); 
            def.OnEnd(ctx);
        }
        catch (OperationCanceledException)
        {
            def.OnCancel(ctx);
        }
        finally
        {
            cts.Dispose();
            cts = null;
        }
    }

    public void Cancel() => cts?.Cancel();
}
