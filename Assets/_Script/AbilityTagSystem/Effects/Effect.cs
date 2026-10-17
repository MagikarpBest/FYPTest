using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class Effect 
{
    public readonly EffectDefinitionSO Definition;

    public bool IsActive;

    public float durationTimer;
    public float periodTimer;

    private Stats stats;
    
    public Effect(EffectDefinitionSO definition, Stats stats)
    {
        Definition = definition;
        this.stats = stats;
        IsActive = false;

        if (Definition.Lifetime == EffectLifetime.Instant)
        {
            stats.TryApplyModifiers(definition.StatModifiers);
            return;
        }
        
        durationTimer = Definition.Duration;
        periodTimer = Definition.ApplyOnFirstTickOfPeriod ? Definition.Period : 0f;

        //modifiers only work if no periodic
        if (definition.Period <= 0f)
        {
            foreach (StatModifier modifier in Definition.StatModifiers)
            {
                stats.AddModifier(modifier);
            }
        }
        
        IsActive = true;
    }

    public void Tick(float deltaTime)
    {
        if (!IsActive) return;
        
        //period
        if (Definition.Period > 0f)
        {
            periodTimer += deltaTime;
            while (periodTimer >= Definition.Period)
            {
                periodTimer -= Definition.Period;
                stats.TryApplyModifiers(Definition.StatModifiers);
            }
        }

        //duration
        if (Definition.Lifetime == EffectLifetime.Duration)
        {
            durationTimer -= deltaTime;
            if (durationTimer <= 0)
            {
                End();
            }
        }
    }

    public void End()
    {
        if (!IsActive) return;
        
        if (Definition.Lifetime != EffectLifetime.Instant && Definition.Period <= 0f)
        {
            foreach (StatModifier modifier in Definition.StatModifiers)
            {
                stats.RemoveModifier(modifier);
            }
        }
        
        IsActive = false;
    }
}
