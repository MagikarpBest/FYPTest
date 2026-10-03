using UnityEngine;

public class EnemySensor : MonoBehaviour
{
    [SerializeField] private bool isDebug = false;
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private LayerMask playerLayer;

    public Transform Target { get; private set; }
    public bool HasTarget => Target != null;
    public float DistanceToTarget { get; private set; }

    private void Update()
    {
        //todo dont let search happen every frame cuz it cost performance make it a scan every ~ second
        // or just a collider detection ontriggerentered
        FindTarget();
        DistanceCheck();
    }

    private void DistanceCheck()
    {
        if (Target != null)
        {
            DistanceToTarget = Vector3.Distance(transform.position, Target.position);
        }
        else
        {
            DistanceToTarget = float.MaxValue;
        }
    }

    private void FindTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);

        if (hits.Length > 0)
        {
            Target = hits[0].transform;
        }
        else
        {
            Target = null;
        }
    }

    private void OnDrawGizmos()
    {
        if (isDebug)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}
