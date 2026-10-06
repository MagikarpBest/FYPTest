using UnityEngine;

[RequireComponent(typeof(PlayerStats))]
public class PlayerDamageReceiver : MonoBehaviour
{
    private PlayerStats _playerStats;

    private void Awake()
    {
        _playerStats = GetComponent<PlayerStats>();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        IDamageSource damageable = hit.collider.GetComponentInParent<IDamageSource>();

        if (damageable == null)
            return;

        _playerStats.TakeDamage(new DamageData(1, DamageType.Physical, AttackPowerLevel.Normal));
    }

#if UNITY_EDITOR
    [InspectorButton("DEBUG: Receive 1 damage", true)] // Only show button in play mode
    private void TestMethod()
    {
        _playerStats.TakeDamage(new DamageData(1, DamageType.Physical, AttackPowerLevel.Normal));
    }
#endif
}
