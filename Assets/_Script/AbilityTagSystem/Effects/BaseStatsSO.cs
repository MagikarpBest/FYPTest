using System.Collections.Generic;
using UnityEngine;

public enum StatType
{
    Health,
    MaxHealth,
    Mana,
    MaxMana,
    Stamina,
    MaxStamina,
    Gold
    
}
public class BaseStatsSO : ScriptableObject
{
    [SerializeField] private List<Stat> stats = new List<Stat>();
    public List<Stat> Stats => stats;
    
    public virtual float PreStatChange(Dictionary<StatType, float> statsDict, StatType statType, float value)
    {
        switch (statType)
        {
            case StatType.Health:
                if (statsDict.TryGetValue(StatType.MaxHealth, out float maxHealth))
                {
                    return Mathf.Clamp(value, 0f, maxHealth);
                }
                break;
            
            case StatType.Mana:
                if (statsDict.TryGetValue(StatType.MaxMana, out float maxMana))
                {
                    return Mathf.Clamp(value, 0f, maxMana);
                }
                break;
            
            case StatType.Stamina:
                if (statsDict.TryGetValue(StatType.MaxStamina, out float maxStamina))
                {
                    return Mathf.Clamp(value, 0f, maxStamina);
                }
                break;
        }
        return value;
    }
}
