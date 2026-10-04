using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class Bomb : MonoBehaviour
{
    [Header("Reference")]
    private Rigidbody rb;
    private Collider col;

    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionForce = 5f;
    private bool _hasExploded;

    private void Awake()
    {
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
    }

    public void Init(float explosionRadius, float explosionForce)
    {
        this.explosionRadius = explosionRadius;
        this.explosionForce = explosionForce;
    }
    public void Hold(Transform bombHoldPoint)
    {
        col.enabled = false;
        rb.isKinematic = true;
        transform.SetParent(bombHoldPoint);
        
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        
        Debug.Log("Holding bomb");
    }

    public void Throw(Vector3 throwDirection, float throwForce, float upwardForce, float explodeDelay = 2.5f)
    {
        transform.SetParent(null);
        rb.isKinematic = false;
        col.enabled = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        Vector3 force = throwDirection.normalized * throwForce + Vector3.up * upwardForce;

        rb.AddForce(force, ForceMode.VelocityChange);
        StartCoroutine(DelayedExplode(explodeDelay));
        Debug.Log("Bomb Thrown");
    }

    private IEnumerator DelayedExplode(float delay)
    {
        // TODO: Handle game pause logic
        yield return new WaitForSeconds(delay);
        Explode();
    }

    public void Explode()
    {
        if (_hasExploded)
            return;

        _hasExploded = true;

        Collider[] objects = Physics.OverlapSphere(transform.position, explosionRadius);
        DamageData damageData = new DamageData(15.0f, DamageType.Explosion, AttackPowerLevel.Explosion);

        foreach (var col in objects)
        {
            Rigidbody targetRb = col.GetComponent<Rigidbody>();
            ILaunchable launchable = col.GetComponent<ILaunchable>();
            // TODO: consider to remove IDestroyable, IDamageable now applicable to map object as in Destructible.cs
            IDestroyable destroyable = col.GetComponent<IDestroyable>(); 
            IDamageable damageable = col.GetComponent<IDamageable>();

            if (launchable != null)
            {
                Vector3 direction = (col.transform.position - transform.position).normalized;
                launchable.Launch(direction * explosionForce);
            }
            else if (destroyable != null)
            {
                destroyable.DestroySelf();
            }
            else if (targetRb != null)
            {
                targetRb.AddExplosionForce(explosionForce, transform.position, explosionRadius, 2f, ForceMode.Impulse);
            }

            if (damageable != null)
            {
                damageable.TakeDamage(damageData);
            }
            
        }
        Destroy(gameObject);
        Debug.Log("Bomb exploded");
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

}
