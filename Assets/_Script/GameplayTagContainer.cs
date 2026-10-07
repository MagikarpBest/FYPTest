using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameplayTagContainer
{
    [SerializeField] private List<GameplayTag> tags = new List<GameplayTag>();
    
    public int Count => tags.Count;

    public void AddTag(GameplayTag tag)
    {
        if (tag.IsValid && !tags.Contains(tag))
        {
            tags.Add(tag);
        }
    }

    public void AddTag(string tag)
    {
        AddTag(new GameplayTag(tag));
    }

    public void RemoveTag(GameplayTag tag)
    {
        tags.Remove(tag);
    }

    public void RemoveTag(string tag)
    {
        RemoveTag(new GameplayTag(tag));
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

    public bool HasTag(string tag)
    {
        return HasTag(new GameplayTag(tag));
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

    public bool HasTagExact(string tag)
    {
        return HasTagExact(new GameplayTag(tag));
    }
}

