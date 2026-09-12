using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] Rigidbody rb;

    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionForce = 5f;


    public void Hold(Transform bombHoldPoint)
    {
        rb.isKinematic = true;
        transform.SetParent(bombHoldPoint);
        rb.interpolation = RigidbodyInterpolation.None;
        
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        
        Debug.Log("Holding bomb");
    }

    public void Throw(Vector3 throwDirection, float throwForce, float upwardForce)
    {
        transform.SetParent(null);
        rb.isKinematic = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        Vector3 force = throwDirection * throwForce + Vector3.up * upwardForce;

        rb.AddForce(force, ForceMode.Impulse);
        Debug.Log("Bomb Thrown");
    }

    public void Explode()
    {
        Collider[] objects = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (var col in objects)
        {
            Rigidbody targetRb = col.GetComponent<Rigidbody>();

            if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                ILaunchable launchable = col.GetComponent<ILaunchable>();
                Vector3 direction = col.transform.position - transform.position;
                launchable.Launch(direction * explosionForce + Vector3.up * (explosionForce));
            }
            if (targetRb != null)
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
