using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameplayTagContainer
{
    [SerializeField] private List<GameplayTag> tags = new List<GameplayTag>();
    public IReadOnlyList<GameplayTag> Tags => tags;
    
    public int Count => tags.Count;

    public void Add(GameplayTag tag)
    {
        if (tag.IsValid && !tags.Contains(tag))
        {
            tags.Add(tag);
        }
    }
    
    public void Remove(GameplayTag tag)
    {
        tags.Remove(tag);
    }

    public void Clear()
    {
        tags.Clear();
    }

    //True if any owned tag is the query tag or a child of it
    public bool HasTag(GameplayTag tag)
    {
        foreach (GameplayTag t in tags)
        {
            if (t.IsChildOf(tag))
            {
                return true;
            }
        }

        return false;
    }
    
    //True only if the exact tag was added
    public bool HasTagExact(GameplayTag tag)
    {
        if (!tag.IsValid)
        {
            return false;
        }

        return tags.Contains(tag);
    }
    
    public bool HasAny(GameplayTagContainer other)
    {
        if (other == null) return false;

        foreach (GameplayTag tag in other.tags)
        {
            if (HasTag(tag)) return true;
        }
        return false;
    }
    
    public bool HasAll(GameplayTagContainer other)
    {
        if (other == null) return true;

        foreach (GameplayTag tag in other.tags)
        {
            if (!HasTag(tag)) return false;
        }
        return true;
    }
    
    public void Add(string tag) => Add(new GameplayTag(tag));
    public void Remove(string tag) => Remove(new GameplayTag(tag));
    public bool HasTag(string tag) => HasTag(new GameplayTag(tag));
    public bool HasTagExact(string tag) => HasTagExact(new GameplayTag(tag));
}

