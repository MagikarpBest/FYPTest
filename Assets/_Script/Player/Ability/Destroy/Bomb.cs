using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] Rigidbody rb;

    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionForce = 5f;
    private bool _hasExploded;


    public void Hold(Transform bombHoldPoint)
    {
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

        foreach (var col in objects)
        {
            Rigidbody targetRb = col.GetComponent<Rigidbody>();
            ILaunchable launchable = col.GetComponent<ILaunchable>();

            if (launchable!=null)
            {
                Vector3 direction = (col.transform.position - transform.position).normalized;
                launchable.Launch(direction * explosionForce );
            }
            else if (targetRb != null)
            {
                targetRb.AddExplosionForce(explosionForce, transform.position, explosionRadius, 2f, ForceMode.Impulse);
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
