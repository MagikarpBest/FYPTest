using System;
using System.Threading;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack", menuName = "AbilitySystem/Abilities/Attack")]
public class AttackAbilitySO : AbilityDefinitionSO
{
    [SerializeField] private float firstHitTime = 1f;
    [SerializeField] private float secondHitDelay = 0.5f;
    [SerializeField] private float endDelay = 0.5f;
    
    public override async Awaitable OnActivate(AbilitySystem ctx, CancellationToken token)
    {
        await Awaitable.WaitForSecondsAsync(firstHitTime, token);
        Debug.Log("Atk 1");

        await Awaitable.WaitForSecondsAsync(secondHitDelay, token);
        Debug.Log("Atk 2");

        await Awaitable.WaitForSecondsAsync(endDelay, token);  
    }

    public override void OnEnd(AbilitySystem ctx)
    {
        Debug.Log("Atk ended");
    }

    public override void OnCancel(AbilitySystem ctx)
    {
        Debug.Log("Atk cancelled");
    }
}
