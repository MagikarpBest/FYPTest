using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public abstract class MovementController : MonoBehaviour
{
    public enum MovementMode
    {
        Disabled,
        Normal,
        Override
    }
    
    public MovementMode Mode {get; private set; } = MovementMode.Normal;

    [SerializeField] protected float rotationSmoothTime = 0.05f;
    [SerializeField] protected float groundCheckDistance = 0.5f;
    [SerializeField] protected float groundCheckPosOffset = -0.14f;
    [SerializeField] protected float groundCheckRadius = 0.28f;
    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected float maxSlopeAngle = 45f;
    [SerializeField] protected float moveSpeed = 11f;
    [SerializeField] protected float gravityMultiplier = 2f;
    [SerializeField] protected float jumpHeight = 3f;
    [SerializeField] protected float externalVelocityDecay = 2f;
    [SerializeField] protected float terminalVelocity = 53.0f;

    protected Rigidbody rb;
    protected Animator animator;

    protected Vector3 targetVelocity = Vector3.zero;
    protected Vector3 externalVelocity = Vector3.zero;

    protected float targetYaw;
    protected Vector3 moveDir = Vector3.zero;
    protected float rotationVelocity = 0f;
    protected RaycastHit groundHit;
    protected float cosMaxSlopeAngle;
    
    public bool IsGrounded { get; private set; } = true;
    
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        rb.useGravity = false;
        targetYaw = transform.eulerAngles.y;
        cosMaxSlopeAngle = Mathf.Cos(maxSlopeAngle * Mathf.Deg2Rad);
    }

    protected virtual void Update()
    {
        GroundCheck();
        HandleRotation();
    }

    protected virtual void ApplyVelocity()
    {
        rb.linearVelocity = SlopeCorrection(targetVelocity) + externalVelocity;
        Vector3 horizontalVelocity = new Vector3(targetVelocity.x, 0f, targetVelocity.z);
        animator.SetFloat("Speed", horizontalVelocity.magnitude);
    }
    
    protected virtual void HandleGravity()
    {
        //stop velocity from dropping infinitely while grounded
        if (IsGrounded && targetVelocity.y < 0f)
        {
            targetVelocity.y = -2f; //keeps you pressed onto the ground and slopes
        }
        else
        {
            //apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            targetVelocity.y += Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;
            targetVelocity.y = Mathf.Max(targetVelocity.y, -terminalVelocity);
        }
    }

    protected virtual void HandleRotation()
    { 
        if (moveDir.sqrMagnitude > 0.01f)
        {
            targetYaw = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
        }
        float smoothedYaw = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetYaw, ref rotationVelocity, rotationSmoothTime);
        transform.rotation = Quaternion.Euler(0f, smoothedYaw, 0f);
    }
    
    protected virtual void GroundCheck()
    {
        Vector3 spherePos = transform.position + Vector3.down * groundCheckPosOffset;
        IsGrounded =
            Physics.SphereCast(spherePos, groundCheckRadius, Vector3.down, out groundHit, groundCheckDistance, groundLayer, QueryTriggerInteraction.Ignore)
            && Vector3.Dot(Vector3.up, groundHit.normal) >= cosMaxSlopeAngle;
    }

    protected virtual void HandleExternalVelocity()
    {
        float dampingFactor = Mathf.Max(0f, 1f - externalVelocityDecay * Time.fixedDeltaTime);
        externalVelocity *= dampingFactor;
    }
    
    protected Vector3 SlopeCorrection(Vector3 velocity)
    {
        if (!IsGrounded || velocity.y > 0f) return velocity;
        
        //flat ground
        if (Vector3.Dot(Vector3.up, groundHit.normal) > 0.999f) return velocity;
        
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);
        if (horizontalVelocity.sqrMagnitude < 0.01f) return Vector3.zero; //standing still prevent sliding
        
        return Vector3.ProjectOnPlane(horizontalVelocity, groundHit.normal).normalized * horizontalVelocity.magnitude;
    }

    public void ApplyForce(Vector3 force)
    {
        externalVelocity += new Vector3(force.x, 0f, force.z);
        targetVelocity.y = force.y;
    }

    public void OverrideMovement(Vector3 movement)
    {
        Mode = MovementMode.Override;
        rb.MovePosition(transform.position + movement);
    }

    public void SetMovementMode(MovementMode mode)
    {
        Mode = mode;
    }

    public void DisableMovement()
    {
        Mode = MovementMode.Disabled;
        StopMovement();
    }

    public void StopMovement()
    {
        targetVelocity.x = 0f;
        targetVelocity.z = 0f;
        animator.SetFloat("Speed", 0);
    }
    
    public void Jump()
    {
        if (!IsGrounded || Mode != MovementMode.Normal) return;
        targetVelocity.y = Mathf.Sqrt(2f * (Physics.gravity.magnitude * gravityMultiplier) * jumpHeight);
    }
    
    protected virtual void OnDrawGizmosSelected()
    {
        Vector3 spherePos = transform.position + Vector3.down * groundCheckPosOffset;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(spherePos, groundCheckRadius);

        // sphere at the end of the cast (max distance traveled)
        Gizmos.color = IsGrounded? Color.green: Color.red;
        Gizmos.DrawWireSphere(spherePos + Vector3.down * groundCheckDistance, groundCheckRadius);

        Debug.DrawLine(spherePos, spherePos + Vector3.down * groundCheckDistance, Color.cyan);
    }
    
    
   
}
