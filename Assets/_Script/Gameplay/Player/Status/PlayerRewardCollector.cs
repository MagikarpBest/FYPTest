using System;

public class PlayerRewardCollector : IDisposable
{
    private readonly PlayerStats _stats;

    public PlayerRewardCollector(PlayerStats stats)
    {
        _stats = stats;
        DeathEvents.OnDeath += HandleDeath;
    }

    public void Dispose() => DeathEvents.OnDeath -= HandleDeath;

    private void HandleDeath(DeathInfo info)
    {
        if (_stats.CurrentHealth <= 0) return;
        _stats.AddMana(info.Reward.ManaValue);
        _stats.AddCurrency(info.Reward.Currency);
    }
}