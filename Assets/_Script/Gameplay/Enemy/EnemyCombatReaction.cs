using System;
using System.Collections;
using UnityEngine;
public enum EnemyReactionType
{
    None,
    Hit,
    Stagger,
    Launch,
    Dead
}
public class EnemyCombatReaction : MonoBehaviour
{
    public event Action<EnemyReactionType> OnReactionRequested;

    private EnemyStats stats;
    private Animator animator;
    public void Init(EnemyStats enemyStats, Animator ani)
    {
        stats = enemyStats;
        animator = ani;
    }

    public void OnDamaged(DamageData damageData)
    {
        if (stats.IsDead)
        {
            OnReactionRequested?.Invoke(EnemyReactionType.Dead);
            return;
        }

        switch (damageData.PowerLevel)
        {
            case AttackPowerLevel.Heavy:
                OnReactionRequested?.Invoke(EnemyReactionType.Launch);
                break;

            default:
                OnReactionRequested?.Invoke(EnemyReactionType.Hit);
                break;
        }
    }
    
    public void HitAnimation()
    {
        animator.SetTrigger("HitTrigger");
        StartCoroutine(ConsumeReaction());
        Debug.Log("enemy hit");
    }
    
    public void LaunchAnimation()
    {
        Debug.Log("enemy launched");
    } 
    
    public void DeadAnimation()
    {
        Debug.Log("enemy dead");
    }

    // private void ConsumeReaction()
    // {
    //     OnReactionRequested?.Invoke(EnemyReactionType.None);
    // }
    
        
    // animation call this when finished
    public IEnumerator ConsumeReaction()
    {
        yield return new WaitForSeconds(1.5f);
        OnReactionRequested?.Invoke(EnemyReactionType.None);
    
    }
}
