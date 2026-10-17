using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameplayTagCountContainer : ISerializationCallbackReceiver
{
    private Dictionary<GameplayTag, int> counts = new Dictionary<GameplayTag, int>();
    [SerializeField] private GameplayTagContainer tags = new GameplayTagContainer(); 
    
    public int Count => tags.Count;
    
    public event Action<GameplayTag, int> OnCountChanged;
    
    public int GetTagCount(GameplayTag tag) => counts.TryGetValue(tag, out int c) ? c : 0;

    public void Add(GameplayTag tag)
    {
        if (!tag.IsValid) return;

        int newCount = GetTagCount(tag) + 1;
        counts[tag] = newCount;
        tags.Add(tag); //already prevents dupes
        
        OnCountChanged?.Invoke(tag, newCount);
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
        
        OnCountChanged?.Invoke(tag, newCount);
    }
    
    public void RemoveAllCountsOfTag(GameplayTag tag)
    {
        if (!counts.ContainsKey(tag)) return;
        
        counts.Remove(tag);
        tags.Remove(tag);
        OnCountChanged?.Invoke(tag, 0);
    }
    
    public void Clear()
    {
        var removed = new List<GameplayTag>(counts.Keys); //snapshot before wiping
        counts.Clear();
        tags.Clear();

        foreach (var tag in removed) OnCountChanged?.Invoke(tag, 0);
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
    
    public void OnBeforeSerialize()
    {
    }
    
    public void OnAfterDeserialize()
    {
        //Drop counts for tags removed in the inspector
        var stale = new List<GameplayTag>();
        foreach (var kvp in counts)
        {
            if (!tags.HasTagExact(kvp.Key)) stale.Add(kvp.Key);
        }
    
        foreach (var tag in stale)
        {
            counts.Remove(tag);
        }
    
        //Give a count of 1 to tags added in the inspector
        foreach (var tag in tags.Tags)
        {
            if (tag.IsValid && !counts.ContainsKey(tag))
            {
                counts[tag] = 1;
                Debug.Log("Tag added from inspector " + tag.Name);
            }
        }
    }
}
