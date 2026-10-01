using System;
using UnityEngine;

public readonly struct RewardInfo
{
    public readonly int Currency;
    public readonly float ManaValue;

    public RewardInfo(float manaValue = 0, int currency = 0)
    {
        Currency = currency;
        ManaValue = manaValue;
    }
}

/// <summary>
/// Struct to pass around information about a death event. Can be used for both player and enemy deaths.
/// Can expand later to include more information like death type (burn, poison, etc.) or define killer information.
/// </summary>
public readonly struct DeathInfo
{
    public readonly Vector3 Position;
    public readonly GameObject Source;
    public readonly RewardInfo Reward;

    public DeathInfo(Vector3 position, GameObject source, RewardInfo reward)
    {
        Position = position;
        Source = source;
        Reward = reward;
    }

    public DeathInfo(Vector3 position, GameObject source, float manaReward = 0f, int currencyReward = 0)
    {
        Position = position;
        Source = source;
        Reward = new RewardInfo(manaReward, currencyReward);
    }
}

/// <summary>
/// Global broadcast for "something died here". Anyone can raise it, anyone can listen.
/// Technically can call DestroyEvents which generally include object and enemy but avoid confusion with Unity method.
/// </summary>
public static class DeathEvents
{
    public static event Action<DeathInfo> OnDeath;
    public static void Raise(DeathInfo info)
    {
        OnDeath?.Invoke(info);
    }

    // With Domain Reload disabled, static state survives between Play sessions.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        OnDeath = null;
    }
}