/*
Use case:
    IDamageable damageable = hit.GetComponent<IDamageable>();
    
    DamageData damageData = new DamageData(
        damage,
        DamageType.Physical,
        AttackPowerLevel.Heavy
    );

    damageable.TakeDamage(damageData);
    
    // Optional:
    DamageResult result = damageable.TakeDamage(damageData);
    switch(result)
    {
        DamageResult.Blocked: Stun player etc.
        default: ignore.
    }
*/

using UnityEngine;

public interface IDamageable
{
    DamageResult TakeDamage(DamageData damageData);
}

/*
enum DamageType
enum AttackPowerLevel
enum DamageResult
struct DamageData
*/
#region Declared Data Type for IDamagable
public enum DamageType
{
    Physical,
    Explosion
}

public enum AttackPowerLevel
{
    Normal = 1,
    Heavy = 2,
    Explosion = 3
}

public enum DamageResult
{
    Ignored,
    Blocked,
    Damaged,
    Destroyed
}

// i changed so its easier to edit damage
[System.Serializable]
public class DamageConfig
{
    public float Damage;
    public DamageType DamageType;
    public AttackPowerLevel PowerLevel;
}

public readonly struct DamageData
{
    public readonly float Damage;
    public readonly DamageType DamageType;
    public readonly AttackPowerLevel PowerLevel;
    public readonly Vector3 SourcePosition;

    public DamageData(
        float damage,
        DamageType damageType,
        AttackPowerLevel powerLevel)
        : this(damage, damageType, powerLevel, Vector3.zero)
    {
    }
    
    public DamageData(
        float damage,
        DamageType damageType,
        AttackPowerLevel powerLevel,
        Vector3 sourcePosition)
    {
        Damage = damage;
        DamageType = damageType;
        PowerLevel = powerLevel;
        SourcePosition = sourcePosition;
    }
}
#endregion