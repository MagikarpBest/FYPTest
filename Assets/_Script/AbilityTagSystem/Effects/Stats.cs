using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stat //because i cant serialize dictionaries.
{
    public StatType Type;
    public float Value;
}

public class Stats 
{
    private readonly BaseStatsSO baseStats;
    private readonly Dictionary<StatType, float> stats = new Dictionary<StatType, float>();
    private List<StatModifier> modifiers = new List<StatModifier>();

    public event Action<StatType, float> OnStatsChange;
    
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
        return stats.TryGetValue(statType, out value);
    }

    public bool TrySetValue(StatType statType, float value)
    {
        if (!stats.ContainsKey(statType)) return false;
        
        value = baseStats.PreStatChange(stats, statType, value);
        stats[statType] = value;
        OnStatsChange?.Invoke(statType, value);
        return true;
    }
    
    public void AddModifier(StatModifier modifier)
    {
        
    }

    public void RemoveModifier(StatModifier modifier)
    {
        
    }
}
