using System;
using UnityEngine;

public class EnemyStatus : MonoBehaviour, IDamageable
{
    // move stats to enemy status
    [Header("Health")]
    [SerializeField] private float maxHealth = 10f;
    private float currentHealth;

    [Header("Hit setting")]
    [SerializeField] private float invincibilityDuration = 1f;
    private float invincibilityTimer;
    
    public float CurrentHealth => currentHealth;

    // Event
    public event Action<float> OnEnemyHealthChange;

    private void Awake()
    {
        currentHealth = maxHealth;
    }
    

    public DamageResult TakeDamage(DamageData damageData)
    {
        // if (currentHealth <= 0)
        // {
        //     // count as death for now
        //     Destroy(gameObject);
        //     return DamageResult.Ignored;
        // }

        float damage = damageData.Damage;
        Debug.Log("Damage receivedw hit");  
        currentHealth -= damage;
        // trigger animation after take damage
        OnEnemyHealthChange?.Invoke(damage);
        return DamageResult.Damaged;
    }
}
