using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Effect", menuName = "AbilitySystem/Effect")]
public class EffectDefinition : ScriptableObject
{
    public enum EffectLifetime
    {
        Instant,
        Duration,
        Infinite
    }
    
    [SerializeField] private EffectLifetime lifetime;
    [SerializeField] private float duration;
    [SerializeField] private List<StatModifier> statModifiers;
    [SerializeField] private float period; //for dot effects
    [SerializeField] private bool applyOnFirstTickOfPeriod; //example if 2 sec period either start apply then or wait the first period before apply 

    private List<StatModifier> StatModifiers => statModifiers;
    public EffectLifetime Lifetime => lifetime;
    public float Period => period;
    
    
    public void Tick(float deltaTime)
    {
        
    }
}
