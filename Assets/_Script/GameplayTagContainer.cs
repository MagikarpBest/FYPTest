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

    public void RemoveTag(GameplayTag tag)
    {
        tags.Remove(tag);
    }

    public void Clear()
    {
        tags.Clear();
    }

    //True if any owned tag is the query tag or a child of it
    public bool HasTag(GameplayTag query)
    {
        foreach (GameplayTag t in tags)
        {
            if (t.IsChildOf(query))
            {
                return true;
            }
        }
        return false;
    }

    //True only if the exact tag was added
    public bool HasTagExact(GameplayTag query)
    {
        if (!query.IsValid)
        {
            return false;
        }
        return tags.Contains(query);
    }
}
