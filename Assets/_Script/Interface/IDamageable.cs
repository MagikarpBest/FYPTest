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

public readonly struct DamageData
{
    public readonly float Damage;
    public readonly DamageType DamageType;
    public readonly AttackPowerLevel PowerLevel;

    public DamageData(
        float damage,
        DamageType damageType,
        AttackPowerLevel powerLevel)
    {
        Damage = damage;
        DamageType = damageType;
        PowerLevel = powerLevel;
    }
}
#endregion