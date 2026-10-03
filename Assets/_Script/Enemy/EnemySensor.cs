using UnityEngine;

public class EnemySensor : MonoBehaviour
{
    [SerializeField] private bool isDebug = false;
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private LayerMask playerLayer;

    private Transform target;

    public Transform Target => target;
    public bool HasTarget => target != null;

    private void Update()
    {
        //todo dont let search happen every frame cuz it cost performance make it a scan every ~ second
        // or just a collider detection ontriggerentered
        FindTarget();
    }
    public float DistanceToTarget()
    {
        return (Vector3.Distance(transform.position, target.position));
    }

    private void FindTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);

        if (hits.Length > 0)
        {
            target = hits[0].transform;
        }
        else
        {
            target = null;
        }
    }

    private void OnDrawGizmos()
    {
        if(isDebug)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}
