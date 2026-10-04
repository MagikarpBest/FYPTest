using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    private Rigidbody[] rbs;
    private SkinnedMeshRenderer renderer;
    
    private void Awake()
    {
        rbs = GetComponentsInChildren<Rigidbody>();
        renderer = GetComponent<SkinnedMeshRenderer>();
    }

    public void TransferVelocity(Vector3 velocity, Vector3 angularVelocity)
    {
        foreach (Rigidbody rb in rbs)
        {
            rb.linearVelocity = velocity;
            rb.angularVelocity = angularVelocity;
        }
    }
}
