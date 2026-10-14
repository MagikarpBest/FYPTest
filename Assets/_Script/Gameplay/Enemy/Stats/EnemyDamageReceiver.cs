using UnityEngine;
using System.Collections.Generic;


public class EnemyDamageReceiver : MonoBehaviour, IDamageable
{
    private EnemyStats stats;
    private EnemyCombatReaction reaction;

    private List<IDefenseLayer> defenses = new();


    public void Init(EnemyStats enemyStats, EnemyCombatReaction combatReaction)
    {
        stats = enemyStats;
        reaction = combatReaction;
        // Find shields, armor, etc
        foreach (var defense in GetComponents<IDefenseLayer>())
        {
            defenses.Add(defense);
        }
    }

    // in future if have more than 1 defense/ buff or whatever this is the place where damage result get procesed
    public DamageResult TakeDamage(DamageData damageData)
    {
        foreach (var defense in defenses)
        {
            DamageResult result = defense.ProcessHit(ref damageData);


            if (result == DamageResult.Blocked ||
                result == DamageResult.Ignored)
            {
                Debug.Log("blocked");
                return result;
            }
        }

        Debug.Log("damaged");
        // status doesnt need to know about what damage result, it should be in EnemyCombatReaction or state machine or mode controller whatever stuff
        stats.ReceiveDamage(damageData.Damage);
        // reaction plays animation stuff
        reaction.OnDamaged(damageData);
        return DamageResult.Damaged;
    }
}
