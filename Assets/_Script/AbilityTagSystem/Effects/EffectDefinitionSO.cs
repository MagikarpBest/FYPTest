using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Effect", menuName = "AbilitySystem/Effect")]
public class EffectDefinitionSO : ScriptableObject
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

    public EffectLifetime Lifetime => lifetime;
    public float Duration => duration;
    public List<StatModifier> StatModifiers => statModifiers;
    public bool ApplyOnFirstTickOfPeriod => applyOnFirstTickOfPeriod;
    public float Period => period;
    
    
    public void Tick(float deltaTime)
    {
        
    }
}
