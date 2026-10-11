using DG.Tweening;
using System;
using UnityEngine;

/// <summary>
/// Rock summoned by RockSkillHandler.
/// Flow: Materialize -> Hover -> Fall -> (Land | Crack)
///   Land  : hits flat ground  -> full AoE, rock stays as obstacle, then shrinks away.
///   Crack : hits an enemy, or an edge / slope -> reduced AoE, rock shatters into debris immediately.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class RockBehaviour : MonoBehaviour, ISummonedObject
{
    [Header("Impact")]
    [Tooltip("Ignore tiny bumps; only count as impact above this speed.")]
    [SerializeField] private float _minImpactSpeed = 2f;
    [SerializeField] private float _shrinkDuration = 0.5f;

    [Header("Land / Crack Rules")]
    [SerializeField] private bool _crackOnDamageable = true; // Hitting anything damageable (enemy) cracks the rock.
    [SerializeField] private float _crackPowerMultiplier = 0.6f; // Damage/force multiplier when the rock cracks instead of landing cleanly.

    private Rigidbody _rb;
    private Collider _col;

    private DamageData _damageData;
    private float _impactRadius;
    private float _impactForce;
    private float _initialFallSpeed;

    private Vector3 _originalScale;
    private bool _isFalling;
    private bool _isShattered;
    private bool _hasImpacted;

    public event Action OnRockMaterializing;
    public event Action OnRockFalling;
    public event Action<Vector3> OnRockLanded;
    public event Action<Vector3> OnRockCracked;
    public event Action<ISummonedObject> OnRemoved;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<Collider>();
        _originalScale = transform.localScale;

        // Frozen in the air until it is told to fall.
        _rb.isKinematic = true;
    }

    public void Init(
        DamageData damageData,
        float impactRadius,
        float impactForce,
        float initialFallSpeed,
        float materializeDuration,
        float hoverDuration)
    {
        _damageData = damageData;
        _impactRadius = impactRadius;
        _impactForce = impactForce;
        _initialFallSpeed = initialFallSpeed;

        PlaySpawnSequence(materializeDuration, hoverDuration);
    }

    public void Shatter()
    {
        if (_isShattered) return;
        _isShattered = true;
        _isFalling = false;
        _hasImpacted = true;

        Crack(transform.position, false);
    }

    private void PlaySpawnSequence(float materializeDuration, float hoverDuration)
    {
        // Don't collide with anything while it's still forming in the air.
        _col.enabled = false;
        transform.localScale = Vector3.zero;

        Sequence sequence = DOTween.Sequence().SetTarget(this);
        sequence.AppendCallback(() => OnRockMaterializing?.Invoke());
        sequence.Append(transform.DOScale(_originalScale, materializeDuration).SetEase(Ease.OutBack));
        sequence.AppendInterval(hoverDuration);
        sequence.AppendCallback(Drop);
    }

    private void Drop()
    {
        _col.enabled = true;
        _rb.isKinematic = false;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // avoid tunneling at high speed
        _rb.linearVelocity = Vector3.down * _initialFallSpeed; // use rb.velocity on Unity versions before 6

        _isFalling = true;
        OnRockFalling?.Invoke();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_isFalling || _hasImpacted)
            return;

        if (collision.relativeVelocity.magnitude < _minImpactSpeed)
            return;

        _hasImpacted = true;
        _isFalling = false;

        Vector3 impactPoint = GetAverageContactPoint(collision);

        if (ShouldCrack(collision))
            Crack(impactPoint);
        else
            Land(impactPoint);
    }

    /// <summary>
    /// Crack if we hit something damageable (enemy) or a non-flat surface (edge, slope, corner).
    /// </summary>
    private bool ShouldCrack(Collision collision)
    {
        if (_crackOnDamageable && HasDamageable(collision.collider) && !IsRock(collision.collider))
            return true;

        return false;
    }

    private static bool HasDamageable(Collider other)
    {
        return other.GetComponentInParent<IDamageable>() != null;
    }

    private static bool IsRock(Collider other)
    {
        return other.GetComponentInParent<RockBehaviour>() != null;
    }

    private static Vector3 GetAverageContactPoint(Collision collision)
    {
        if (collision.contactCount == 0)
            return collision.transform.position;

        Vector3 sum = Vector3.zero;
        for (int i = 0; i < collision.contactCount; i++)
            sum += collision.GetContact(i).point;

        return sum / collision.contactCount;
    }

    private void Land(Vector3 impactPoint)
    {
        // TODO: Apply surrounding force with no damage.
        OnRockLanded?.Invoke(impactPoint);
    }

    private void Crack(Vector3 impactPoint, bool dispose = true)
    {
        Debug.Log("Apply crack");
        ApplyAreaImpact(impactPoint);
        OnRockCracked?.Invoke(impactPoint);

        // TODO: Spawn cracked visual effect? either subscribe to OnRockCracked or add a serialized prefab here.

        if (dispose) Destroy(gameObject);
    }

    /// <summary>
    /// ISummonedObject interface. Called by RockSkillHandler when the rock is no longer needed.
    /// </summary>
    public void Despawn()
    {
        _col.enabled = false;

        Sequence sequence = DOTween.Sequence().SetTarget(this);
        sequence.Append(transform.DOScale(Vector3.zero, _shrinkDuration).SetEase(Ease.InCubic));
        sequence.OnComplete(() => Destroy(gameObject));
    }
    
    private void ApplyAreaImpact(Vector3 impactPoint)
    {
        Collider[] objects = Physics.OverlapSphere(impactPoint, _impactRadius);
        DamageData damageData = new DamageData(_damageData.Damage, _damageData.DamageType, _damageData.PowerLevel);
        float force = _impactForce;

        foreach (Collider other in objects)
        {
            // Skip ourselves.
            if (other.attachedRigidbody == _rb)
                continue;

            IDamageable damageable = other.GetComponent<IDamageable>();
            Rigidbody targetRb = other.attachedRigidbody;

            if (targetRb != null)
            {
                targetRb.AddExplosionForce(force, impactPoint, _impactRadius, 2f, ForceMode.Impulse);
            }

            if (damageable != null)
            {
                damageable.TakeDamage(damageData);
            }
        }
    }

    private void OnDestroy()
    {
        DOTween.Kill(this);
        DOTween.Kill(transform);
        OnRemoved?.Invoke(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _impactRadius);
    }
}