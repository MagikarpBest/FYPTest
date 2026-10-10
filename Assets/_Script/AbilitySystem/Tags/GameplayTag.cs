using System;
using UnityEngine;

[Serializable]
public struct GameplayTag : IEquatable<GameplayTag>
{
    [SerializeField] private string name;
    
    public GameplayTag(string name)
    {
        this.name = name;
    }
    
    public string Name => name == null ? string.Empty : name;
    public bool IsValid => !string.IsNullOrEmpty(name) && GameplayTagDatabase.Contains(this);
    
    public bool Equals(GameplayTag other) => string.Equals(name, other.name, StringComparison.Ordinal);
    
    public bool IsChildOf(GameplayTag other)
    {
        if (!IsValid || !other.IsValid)
        {
            return false;
        }
        return name == other.name || name.StartsWith(other.name + ".", StringComparison.Ordinal);
    }
}
