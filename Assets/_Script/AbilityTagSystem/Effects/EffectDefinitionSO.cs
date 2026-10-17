using System;
using System.Collections.Generic;
using UnityEngine;

public enum EffectLifetime
{
    Instant,
    Duration,
    Infinite
}

[CreateAssetMenu(fileName = "New Effect", menuName = "AbilitySystem/Effect")]
public class EffectDefinitionSO : ScriptableObject
{
    [SerializeField] private EffectLifetime lifetime;
    [SerializeField] private float duration;
    [SerializeField] private List<StatModifier> statModifiers;
    [SerializeField] private float period; //for dot effects
    [SerializeField] private bool applyOnFirstTickOfPeriod; //example if 2 sec period either start apply then or wait the first period before apply 
    [SerializeField] private List<GameplayTag> grantedTags;
    [SerializeField] private List<AbilityDefinitionSO> grantedAbilities;
    [SerializeField] private List<GameObject> vfxs;
    [SerializeField] private List<AudioClip> soundsEffects;
    
    public EffectLifetime Lifetime => lifetime;
    public float Duration => duration;
    public List<StatModifier> StatModifiers => statModifiers;
    public bool ApplyOnFirstTickOfPeriod => applyOnFirstTickOfPeriod;
    public float Period => period;
    public List<GameplayTag> GrantedTags => grantedTags;
    public List<AbilityDefinitionSO> GrantedAbilities => grantedAbilities;
    public List<GameObject> Vfxs => vfxs;
    public List<AudioClip> SoundsEffects => soundsEffects;
}
