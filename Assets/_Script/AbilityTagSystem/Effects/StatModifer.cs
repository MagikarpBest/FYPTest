using UnityEngine;
using System;

[Serializable]
public class StatModifier
{
    public enum StatModifierOperation
    {
        Add,
        Subtract,
        Multiply,
        Override
    }

    public enum ModifierMagnitudeType
    {
        Float,
        SetByCallerUsingTag
    }
    
    public StatType StatType;
    public ModifierMagnitudeType MagnitudeType;
    public StatModifierOperation Operation;

    public float FloatMagnitude;
    public GameplayTag TagMagnitude;
}
