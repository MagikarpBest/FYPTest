using System;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// Use this class for prototype purpose, ideally all the object define their own destroyed behaviour.
/// Normally attach with object that has collider.
/// </summary>
public class Destructible : MonoBehaviour, IDamageable, IHealthSource
{
    [SerializeField] protected float _maxHealth;
    [SerializeField] private AttackPowerLevel _requiredPowerLevel = AttackPowerLevel.Normal;
    [SerializeField] protected float _manaValue = 1f;
    [SerializeField] protected int _currencyValue = 0;

    // Prototype purpose
    [Header("Feedback")]
    [SerializeField] private float _blockedShakeDuration = 0.5f;
    [SerializeField] private float _blockedShakeStrength = 0.1f;
    [SerializeField] private float _damagedPunchDuration = 0.5f;
    [SerializeField] private float _damagedPunchStrength = 0.1f;

    [Header("Destroy")]
    [Tooltip("Spawned (unparented) at the moment of destruction. Leave empty for no particle.")]
    [SerializeField] private GameObject _destroyEffectPrefab;
    [Tooltip("Local-space offset from this object's pivot where the effect spawns.")]
    [SerializeField] private Vector3 _destroyEffectOffset;
    [Tooltip("Seconds before the whole GameObject is removed. Keep it longer than the destroy animation/effect.")]
    [SerializeField] private float _destroyDelay = 3f;

    protected float _currentHealth;

    private bool _isDestroyed;

    public float CurrentHealth => _currentHealth;
    public float MaxHealth => _maxHealth;

    public event Action<float, float> OnHealthChanged;
    public event Action OnDamageBlocked;

    protected virtual void Awake()
    {
        _currentHealth = _maxHealth;
    }

    protected virtual void Start()
    {
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
    }

    public virtual DamageResult TakeDamage(DamageData damageData)
    {
        if (_isDestroyed)
            return DamageResult.Ignored;

        if (!CanReceiveDamage(damageData))
        {
            OnBlocked(damageData);
            OnDamageBlocked?.Invoke();
            return DamageResult.Blocked;
        }

        _currentHealth -= damageData.Damage;
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

        if (_currentHealth <= 0f)
        {
            _currentHealth = 0f;
            _isDestroyed = true;

            OnDestroyed();
            return DamageResult.Destroyed;
        }

        OnDamaged(damageData);
        return DamageResult.Damaged;
    }

    protected virtual void OnBlocked(DamageData damageData)
    {
        // Finish any running feedback first so repeated hits don't leave position/scale drifting.
        DOTween.Complete(this);

        // ChatGPT placeholder
        transform.DOShakePosition(_blockedShakeDuration, _blockedShakeStrength, 20, 90f).SetId(this);
    }

    protected virtual void OnDamaged(DamageData damageData)
    {
        DOTween.Complete(this);

        // ChatGPT placeholder
        transform.DOPunchScale(Vector3.one * _damagedPunchStrength, _damagedPunchDuration).SetId(this);
    }

    // Not the Monobehaviour Lifecycle OnDestroy
    protected virtual void OnDestroyed()
    {
        // Settle any running hit feedback so the object isn't frozen mid-shake.
        DOTween.Complete(this);

        DeathEvents.Raise(new DeathInfo(transform.position, gameObject, _manaValue, _currencyValue));
        
        Destroy(gameObject);
        
        
        SpawnDestroyEffect();
        
        
        //No point 
        // SetCollidersEnabled(false);
        // SetRenderersEnabled(false);
        
        // if (TryGetComponent<Rigidbody>(out var rb))
        // {
        //     rb.isKinematic = true;
        // }

        // Remove the whole thing after the effect has had time to finish.
        //Destroy(gameObject, _destroyDelay);
    }

    protected virtual void SpawnDestroyEffect()
    {
        
        
        
        //if (_destroyEffectPrefab == null) return
        //Vector3 position = transform.TransformPoint(_destroyEffectOffset);
        
        //VFX Should be its own script and own pooling
        // ParticleSystem effect = Instantiate(_destroyEffectPrefab, position, transform.rotation);
        //
        // ParticleSystem.MainModule main = effect.main;
        // float lifetime = main.duration + main.startLifetime.constantMax;
        //
        // effect.Play();
        // Destroy(effect.gameObject, lifetime);
        //Dont handle destruction here
        //Obj should always handle their own destruction like vfx handle their own cleanup 
        //Destruction prefab handles their own cleanup
    }

    // Set exception when needed like:
    // {
    //      if (this.magicBarrier is active) return false
    //      base();
    // }
    protected virtual bool CanReceiveDamage(DamageData damageData)
    {
        return damageData.PowerLevel >= _requiredPowerLevel;
    }

    protected virtual void OnDestroy()
    {
        DOTween.Kill(this);
        transform.DOKill();
    }

    private void SetCollidersEnabled(bool enabled)
    {
        foreach (Collider col in GetComponentsInChildren<Collider>())
            col.enabled = enabled;

        foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
            col.enabled = enabled;
    }

    private void SetRenderersEnabled(bool enabled)
    {
        foreach (Renderer rend in GetComponentsInChildren<Renderer>())
            rend.enabled = enabled;
    }
}