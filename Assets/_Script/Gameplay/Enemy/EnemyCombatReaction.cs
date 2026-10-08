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
    public EnemyReactionType PendingReaction { get; private set; } = EnemyReactionType.None;
    public bool HasPendingReaction { get; private set; }
    public DamageData LastHit { get; private set; }

    private EnemyStats stats;
    private Animator animator;
    public void Init(EnemyStats enemyStats, Animator ani)
    {
        stats = enemyStats;
        animator = ani;
    }

    public void OnDamaged(DamageData damageData)
    {
        LastHit = damageData;
        if(stats.IsDead)
        {
            PendingReaction = EnemyReactionType.Dead;
        }

        switch (damageData.PowerLevel)
        {
            case AttackPowerLevel.Normal:
                PendingReaction = EnemyReactionType.Hit;
                break;
            
            case AttackPowerLevel.Heavy:
                PendingReaction = EnemyReactionType.Launch;
                break;
            
            default:
                PendingReaction = EnemyReactionType.Hit;
                break;
        }
        HasPendingReaction = true;
    }
    
    public void HitAnimation()
    {
        //animator.SetTrigger("Hit");
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
    
    // animation call this when finished
    public IEnumerator ConsumeReaction()
    {
        yield return new WaitForSeconds(1.5f);
        PendingReaction = EnemyReactionType.None;
    }
}
