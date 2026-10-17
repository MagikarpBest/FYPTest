using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stat //because i cant serialize dictionaries.
{
    public StatType Type;
    public float Value;
}

public class StatQuery
{
    public StatType StatType;
    private float value;
    public float Value => overrideValue ?? ((value + addBase) * multiply / divide + addFinal);

    public StatQuery(StatType statType, float value)
    {
        StatType = statType;
        this.value = value;
    }
    
    private float addBase;
    private float addFinal;
    private float multiply = 1f;
    private float divide = 1f;
    private float? overrideValue;

    public void ApplyModifier(StatModifierOperation operation, float magnitude)
    {
        switch (operation)
        {
            case StatModifierOperation.AddBase: addBase += magnitude; break;
            case StatModifierOperation.AddFinal: addFinal += magnitude; break;
            case StatModifierOperation.Multiply: multiply *= magnitude; break;
            case StatModifierOperation.Divide: if (magnitude != 0f) divide *= magnitude; break;
            case StatModifierOperation.Override: overrideValue = magnitude; break;
        }
    }
}

public class Stats 
{
    private readonly BaseStatsSO baseStats;
    private readonly Dictionary<StatType, float> stats = new Dictionary<StatType, float>();
    private List<StatModifier> modifiers = new List<StatModifier>();

    public event Action<StatQuery> OnStatsQueried;
    public event Action<StatType,float> OnStatsChange;
    
    public Stats(BaseStatsSO baseStats)
    {
        this.baseStats = baseStats;
        foreach (Stat stat in baseStats.Stats)
        {
            stats[stat.Type] = stat.Value;
        }
    }
    
    public bool TryGetValue(StatType statType, out float value)
    {
        if (stats.TryGetValue(statType, out value)) //value is copy here
        {
            StatQuery query = new StatQuery(statType, value);
            OnStatsQueried?.Invoke(query); //modifiers are subs to this and will modify the value
            value = query.Value; //modified value
            return true;
        }
        return false;
    }

    public bool TrySetValue(StatType statType, float value)
    {
        if (!stats.ContainsKey(statType)) return false;
        
        value = baseStats.PreStatChange(stats, statType, value);
        stats[statType] = value;
        NotifyStatsChange(statType);
        return true;
    }

    public void AddModifier(StatModifier modifier)
    {
        modifiers.Add(modifier);
        OnStatsQueried += modifier.Handle;
        NotifyStatsChange(modifier.StatType);
    }
    
    public void RemoveModifier(StatModifier modifier)
    {
        OnStatsQueried -= modifier.Handle;
        modifiers.Remove(modifier);
        NotifyStatsChange(modifier.StatType);
    }

    private void NotifyStatsChange(StatType statType)
    {
        if (TryGetValue(statType, out float value))
        {
            OnStatsChange?.Invoke(statType, value);
        }
    }
    public bool TryApplyModifiers(IEnumerable<StatModifier> appliedModifiers)
    {
        var queries = new Dictionary<StatType, StatQuery>();

        foreach (StatModifier modifier in appliedModifiers)
        {
            if (!stats.TryGetValue(modifier.StatType, out float baseValue)) continue;

            if (!queries.TryGetValue(modifier.StatType, out var query))
            {
                query = new StatQuery(modifier.StatType, baseValue);
                queries[modifier.StatType] = query;
            }

            query.ApplyModifier(modifier.Operation, modifier.FloatMagnitude);
        }
        
        if (queries.Count <= 0) return false;
        
        foreach (var kvp in queries)
        {
            TrySetValue(kvp.Key, kvp.Value.Value);
        }

        return true;
    }
}
