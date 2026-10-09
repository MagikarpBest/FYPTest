using System;

/// <summary>
/// Used to group both destructible and enemy with health.
/// So no need redundancy UI code for both destructible and enemy.
/// 
/// the DamageBlocked being declared here can be not used for enemy, but it is used for destructible, so just leave it here.
/// </summary>
public interface IHealthSource
{
    float CurrentHealth { get; }
    float MaxHealth { get; }

    event Action<float, float> OnHealthChanged;
    event Action OnDamageBlocked;
}
