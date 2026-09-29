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

    // Event
    public event Action<float> OnEnemyHealthChange;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float Damage)
    {
        if (currentHealth <= 0)
        {
            return;
        }

        Debug.Log("Damage receivedw hit");  
        currentHealth -= Damage;
        // trigger animation after take damage
        OnEnemyHealthChange?.Invoke(Damage);
    }
}
