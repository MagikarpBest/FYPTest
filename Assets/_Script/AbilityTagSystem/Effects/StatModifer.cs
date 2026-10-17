using UnityEngine;
using System;

public enum StatModifierOperation
{
    AddBase,
    AddFinal,
    Multiply,
    Divide,
    Override
}

public enum ModifierMagnitudeType
{
    Float,
    SetByCallerUsingTag
}

[Serializable]
public class StatModifier
{
    public StatType StatType;
    public ModifierMagnitudeType MagnitudeType;
    public StatModifierOperation Operation;

    public float FloatMagnitude;
    public GameplayTag TagMagnitude;
    
    public void Handle(StatQuery query)
    {
        if (query.StatType != StatType) return;

        float magnitude = 0f;
        if (MagnitudeType == ModifierMagnitudeType.Float)
        {
            magnitude = FloatMagnitude;
        }
        else
        {
            
        }
        
        query.ApplyModifier(Operation, magnitude);
    }
}
