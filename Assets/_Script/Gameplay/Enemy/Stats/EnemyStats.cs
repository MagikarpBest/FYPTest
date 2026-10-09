using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemyStats : MonoBehaviour, IHealthSource
{
    // move stats to enemy status
    [Header("Health")]
    [SerializeField] private float maxHealth = 10f;
    private float currentHealth;

    [Header("Hit setting")]
    [SerializeField] private float invincibilityDuration = 1f;
    private float invincibilityTimer;
    
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    // Event
    public event Action<float, float> OnHealthChanged;
    public event Action OnDamageBlocked; // Required for IHealthSource interface, but not used in EnemyStats

    private void Awake()
    {
        currentHealth = maxHealth;
    }


    // this only - hp, all the logic check enemydamagereceiver
    public void ReceiveDamage(float damage)
    {
        Debug.Log("Damage received hit");  
        
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            // count as death for now
            Die();
        }
        
        // trigger animation after take damage
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    
    private void Die()
    {
        Debug.Log("Enemy Die");
    }
}
