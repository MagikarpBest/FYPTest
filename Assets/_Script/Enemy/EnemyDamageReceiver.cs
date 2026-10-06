using UnityEngine;
using System.Collections.Generic;


public class EnemyDamageReceiver : MonoBehaviour, IDamageable
{
    private EnemyStatus status;

    private List<IDefenseLayer> defenses = new();


    private void Awake()
    {
        status = GetComponent<EnemyStatus>();
        
        // Find shields, armor, etc
        foreach(var defense in GetComponents<IDefenseLayer>())
        {
            defenses.Add(defense);
        }
    }

    // in future if have more than 1 defense/ buff or whatever this is the place where damage result get procesed
    public DamageResult TakeDamage(DamageData damageData)
    {
        foreach(var defense in defenses)
        {
            DamageResult result = defense.ProcessHit(ref damageData);


            if(result == DamageResult.Blocked ||
               result == DamageResult.Ignored)
            {
                return result;
            }
        }


        // status doesnt need to know about what damage result, it sohuld be in EnemyCombatReaction or state machine or mode controller whatever stuff
        status.ReceiveDamage(damageData.Damage);
        return DamageResult.Damaged;
    }
}
