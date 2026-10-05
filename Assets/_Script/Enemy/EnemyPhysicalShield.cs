using UnityEngine;


public class EnemyPhysicalShield : MonoBehaviour, IDefenseLayer
{
    [SerializeField] private Transform owner;
    [SerializeField] private int durability = 5;
    [SerializeField] private float blockAngle = 120f;


    public DamageResult ProcessHit(ref DamageData damageData)

    {
        if (durability <= 0)
        {
            Debug.Log("Shield broke");
            return DamageResult.Damaged;
        }

        durability -= (int)damageData.Damage;
        Vector3 direction = (damageData.SourcePosition - owner.position).normalized;


        float angle = Vector3.Angle(owner.forward, direction);


        if (angle <= blockAngle / 2)
        {
            Debug.Log("Blocked");

            return DamageResult.Blocked;
        }


        return DamageResult.Damaged;
    }
}
