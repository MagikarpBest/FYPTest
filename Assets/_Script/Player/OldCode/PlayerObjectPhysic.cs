using UnityEngine;

public class PlayerObjectPhysic : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float pushForce = 5f;
    
    // have to do this manually because we are using character controller and avoid jitter issue
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rb = hit.collider.attachedRigidbody;
        
        if(rb == null ||rb.isKinematic)
        {
            return;
        }

        // dont add force to object below player
        if (hit.moveDirection.y < -0.3f)
        {
            return;
        }

        Vector3 pushDirection = new Vector3(hit.moveDirection.x, 0.0f, hit.moveDirection.z);
        rb.AddForce(pushDirection*pushForce, ForceMode.Impulse);
    }
}
