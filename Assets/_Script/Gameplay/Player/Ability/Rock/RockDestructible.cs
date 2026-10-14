using UnityEngine;

/// <summary>
/// Rock-only destructible, used to call Crack attack when destructible run out of health due to entities attack.
/// </summary>
[RequireComponent(typeof(RockBehaviour))]
public class RockDestructible : Destructible
{
    private RockBehaviour _rock;

    protected override void Awake()
    {
        base.Awake();
        _rock = GetComponent<RockBehaviour>();
    }

    protected override void OnDestroyed()
    {
        _rock.Shatter();
        base.OnDestroyed();
    }
}