using System;

public class PlayerRewardCollector : IDisposable
{
    private readonly PlayerStatus _status;

    public PlayerRewardCollector(PlayerStatus status)
    {
        _status = status;
        DeathEvents.OnDeath += HandleDeath;
    }

    public void Dispose() => DeathEvents.OnDeath -= HandleDeath;

    private void HandleDeath(DeathInfo info)
    {
        if (_status.CurrentHealth <= 0) return;
        _status.AddMana(info.Reward.ManaValue);
        _status.AddCurrency(info.Reward.Currency);
    }
}