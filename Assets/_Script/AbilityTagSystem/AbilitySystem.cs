using UnityEngine;
using System;
using System.Collections.Generic;

public class AbilitySystem : MonoBehaviour
{
    public Character Character { get; private set; } 
    
    [SerializeField] private GameplayTagCountContainer tags;
    [SerializeField] private List<AbilityDefinitionSO> startingAbilities = new List<AbilityDefinitionSO>();
    [SerializeField] private List<Ability> abilities = new List<Ability>();

    
    public event Action<string> OnAnimationEvent;
    public event Action<GameplayTag, int> OnTagCountChanged;
    public event Action<Ability> OnAbilityGained;
    public event Action<Ability> OnAbilityRemoved;
    public event Action<Ability> OnAbilityActivated;
    public event Action<Ability> OnAbilityFinished;     
    public event Action<Ability> OnAbilityCancelled; 
    
    private void Awake()
    {
        //use this for ref
        //like in abilities call ctx.owner.input,
        //ctx.owner.movement
        //or if unique components do ctx.owner.TryGetComponent
        Character = GetComponent<Character>();
        
        foreach (var def in startingAbilities)
        {
            GiveAbility(def);
        }
    }
    
    private void OnEnable()
    {
        tags.CountChanged += HandleTagCountChanged;
    }

    private void OnDisable()
    {
        tags.CountChanged -= HandleTagCountChanged;
        foreach (Ability ability in abilities)
        {
            CancelAbility(ability);
        }
    }

    private void OnDestroy() => CancelAllAbilities();
    
    public void NotifyAnimationEvent(string tag) => OnAnimationEvent?.Invoke(tag);
    private void HandleTagCountChanged(GameplayTag tag, int newCount) => OnTagCountChanged?.Invoke(tag, newCount);
    public void NotifyAbilityActivated(Ability ability) => OnAbilityActivated?.Invoke(ability);
    public void NotifyAbilityCancelled(Ability ability) => OnAbilityCancelled?.Invoke(ability);
    public void NotifyAbilityFinished(Ability ability) => OnAbilityFinished?.Invoke(ability);
    
    #region GameplayTags
    public int GetTagCount(GameplayTag tag) => tags.GetTagCount(tag);
    public void AddTag(GameplayTag tag) => tags.Add(tag);
    public void RemoveTag(GameplayTag tag) => tags.Remove(tag);
    public bool HasTag(GameplayTag tag) => tags.HasTag(tag);
    public bool HasTagExact(GameplayTag tag) => tags.HasTagExact(tag);
    public bool HasAnyTags(GameplayTagContainer c) => tags.HasAny(c);
    public bool HasAllTags(GameplayTagContainer c) => tags.HasAll(c);
    public void ClearAllTags() => tags.Clear();
    
    public void AddTags(GameplayTagContainer c)
    {
        foreach (GameplayTag tag in c.Tags) tags.Add(tag);
    }

    public void RemoveTags(GameplayTagContainer c)
    {
        foreach (GameplayTag tag in c.Tags) tags.Remove(tag);
    }
    
    public bool AddTagUnique(GameplayTag tag)
    {
        if (tags.HasTagExact(tag))
        {
            return false; //already there, do nothing
        }

        tags.Add(tag);
        return true;
    }
    
    public void RemoveAllCountsOfTag(GameplayTag tag) => tags.RemoveAllCountsOfTag(tag);
    
    //string overloads
    public void RemoveAllCountsOfTag(string tag) => tags.RemoveAllCountsOfTag(new GameplayTag(tag));
    public bool AddTagUnique(string tag) => AddTagUnique(new GameplayTag(tag));
    public int GetTagCount(string tag) => tags.GetTagCount(tag);
    public void AddTag(string tag) => tags.Add(tag);
    public void RemoveTag(string tag) => tags.Remove(tag);
    public bool HasTag(string tag) => tags.HasTag(tag);
    public bool HasTagExact(string tag) => tags.HasTagExact(tag);
    #endregion
    
    #region Abilities
    public Ability GiveAbility(AbilityDefinitionSO abilityDef)
    {
        if (abilityDef == null) return null;

        Ability existing = abilities.Find(a => a.Definition == abilityDef);
        if (existing != null) return existing;

        Ability ability = new Ability(abilityDef, this);
        abilities.Add(ability);
        OnAbilityGained?.Invoke(ability);
        return ability;
    }
    
    public void RemoveAbility(Ability ability)
    {
        if (ability == null) return;
        if (!abilities.Remove(ability)) return;  

        ability.Cancel(); //cancel if it running
        OnAbilityRemoved?.Invoke(ability);
    }

    public void RemoveAbility(AbilityDefinitionSO def)
    {
        RemoveAbility(abilities.Find(a => a.Definition == def));
    }
    
    public bool AreAbilityTagsBlocked(GameplayTagContainer abilityTags)
    {
        foreach (Ability ability in abilities)
        {
            if (!ability.IsRunning) continue;
            if (abilityTags.HasAny(ability.Definition.BlockAbilitiesWithTag)) return true;
        }
        return false;
    }

    public bool CanActivateAbility(Ability ability)
    {
        return ability != null && ability.CanActivateAbility();
    }

    public bool TryActivateAbility(Ability ability)
    {
        if (!CanActivateAbility(ability)) return false;
        
        CancelAbilitiesWithTags(ability.Definition.CancelAbilitiesWithTag, except: ability);
        
        ability.Activate();
        return true;
    }

    public bool TryActivateAbilityWithTag(GameplayTag tag)
    {
        foreach (Ability ability in abilities)
        {
            if (!ability.Definition.AbilityTags.HasTagExact(tag)) continue;
            if (TryActivateAbility(ability)) return true;
        }
        return false;
    }
    
    public bool TryActivateAbilityWithTag(string tag) => TryActivateAbilityWithTag(new GameplayTag(tag));

    public void CancelAbility(Ability ability)
    {
        ability?.Cancel();
    }

    public void CancelAbilitiesWithTag(GameplayTag tag, Ability except = null)
    {
        foreach (Ability ability in abilities)
        {
            if (ability == except || !ability.IsRunning) continue;

            if (ability.Definition.AbilityTags.HasTag(tag))
            {
                ability.Cancel();
            }
        }
    }

    public void CancelAbilitiesWithTag(string tag, Ability except = null) => CancelAbilitiesWithTag(new GameplayTag(tag), except);

    public void CancelAbilitiesWithTags(GameplayTagContainer c, Ability except = null)
    {
        foreach (GameplayTag tag in c.Tags)
        {
            CancelAbilitiesWithTag(tag, except);
        }
    }
    
    public void CancelAllAbilities()
    {
        foreach (Ability ability in abilities)
        {
            ability.Cancel();
        }
    }
    
    #endregion

    #region Effects

    #endregion
}
