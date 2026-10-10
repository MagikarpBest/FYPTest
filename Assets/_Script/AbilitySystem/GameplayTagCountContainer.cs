using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameplayTagCountContainer
{
    private Dictionary<GameplayTag, int> counts = new Dictionary<GameplayTag, int>();
    [SerializeField] private GameplayTagContainer tags = new GameplayTagContainer(); 
    
    public int Count => tags.Count;
    
    public event Action<GameplayTag, int> CountChanged;
    
    public int GetTagCount(GameplayTag tag) => counts.TryGetValue(tag, out int c) ? c : 0;

    public void Add(GameplayTag tag)
    {
        if (!tag.IsValid) return;

        int newCount = GetTagCount(tag) + 1;
        counts[tag] = newCount;
        tags.Add(tag); //already prevents dupes
        
        CountChanged?.Invoke(tag, newCount);
    }

    public void Remove(GameplayTag tag)
    {
        int count = GetTagCount(tag);
        if (count == 0) return;

        int newCount = count - 1;
        if (newCount == 0)
        {
            counts.Remove(tag);
            tags.Remove(tag);
        }
        else
        {
            counts[tag] = newCount;
        }
        
        CountChanged?.Invoke(tag, newCount);
    }
    
    public void Clear()
    {
        var removed = new List<GameplayTag>(counts.Keys); //snapshot before wiping
        counts.Clear();
        tags.Clear();

        foreach (var tag in removed) CountChanged?.Invoke(tag, 0);
    }
    
    public bool HasTag(GameplayTag tag) => tags.HasTag(tag);
    public bool HasTagExact(GameplayTag tag) => tags.HasTagExact(tag);
    public bool HasAny(GameplayTagContainer other) => tags.HasAny(other);
    public bool HasAll(GameplayTagContainer other) => tags.HasAll(other);
    
    public int GetTagCount(string tag) => GetTagCount(new GameplayTag(tag));
    public void Add(string tag) => Add(new GameplayTag(tag));
    public void Remove(string tag) => Remove(new GameplayTag(tag));
    public bool HasTag(string tag) => tags.HasTag(tag);
    public bool HasTagExact(string tag) => tags.HasTagExact(tag);
    
    void OnValidate()
    {
        //Drop counts for tags that are no longer in the set (removed in the inspector)
        var stale = new List<GameplayTag>();
        foreach (var kvp in counts)
        {
            if (!tags.HasTagExact(kvp.Key)) stale.Add(kvp.Key);
        }
        
        foreach (var tag in stale)
        {
            counts.Remove(tag);
        }

        //Give a count of 1 to tags in the set that have no count (added in the inspector or restored after a recompile wiped the dictionary)
        foreach (var tag in tags.Tags)
        {
            if (tag.IsValid && !counts.ContainsKey(tag))
            {
                counts[tag] = 1;
            }
        }
    }
}
