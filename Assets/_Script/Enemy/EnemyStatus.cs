using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemyStatus : MonoBehaviour
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
        //OnEnemyHealthChange?.Invoke(damage);
        
    }
    
    private void Die()
    {
        Debug.Log("Enemy Die");
    }
}
