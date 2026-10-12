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

    [SerializeField] protected float rotationSpeed = 727f;
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
    protected AbilitySystem abilitySystem;
    
    protected Vector3 moveVelocity = Vector3.zero;
    protected Vector2 horizontalVelocity => new Vector2(moveVelocity.x, moveVelocity.z);
    private float verticalVelocity => moveVelocity.y;
    
    protected Vector3 externalVelocity = Vector3.zero;
    protected Vector3 gravity = Vector3.zero;

    protected float targetYaw;
    protected RaycastHit groundHit;
    protected float cosMaxSlopeAngle;
    
    public bool IsGrounded { get; private set; } = true;
    public bool IsWalking => IsGrounded && horizontalVelocity.sqrMagnitude > 0.01f;
    public bool IsFalling => !IsGrounded && moveVelocity.y + gravity.y + externalVelocity.y < 0;
    public bool IsJumping { get; private set; } = false;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        rb.useGravity = false;
        targetYaw = transform.eulerAngles.y;
        cosMaxSlopeAngle = Mathf.Cos(maxSlopeAngle * Mathf.Deg2Rad);
        abilitySystem = GetComponent<AbilitySystem>();
    }
    
    private void Update()
    {
        GroundCheck();
        
        if (IsWalking)
        {
            abilitySystem.AddTagUnique("Movement.Walking");
        }
        else
        {
            abilitySystem.RemoveTag("Movement.Walking");
        }

        if (IsGrounded)
        {
            abilitySystem.RemoveTag("State.Airborne");
            abilitySystem.AddTagUnique("State.Grounded");
            
            if (IsJumping && rb.linearVelocity.y < 0f)
            { 
                moveVelocity.y = 0f; 
                IsJumping = false;
            }
        }
        else
        {
            abilitySystem.RemoveTag("State.Grounded");
            abilitySystem.AddTagUnique("State.Airborne");
        }
        
        //animator.SetBool("IsFalling", IsFalling);
    }
    
    protected virtual void ApplyVelocity()
    {
        rb.linearVelocity = SlopeCorrection(moveVelocity) + externalVelocity + gravity;
        animator.SetFloat("Speed",  Mathf.Clamp01(horizontalVelocity.magnitude / moveSpeed));
        //Debug.Log(Mathf.Clamp01(horizontalVelocity.magnitude / moveSpeed));
    }
    
    protected virtual void HandleGravity()
    {
        //stop velocity from dropping infinitely while grounded
        if (IsGrounded && moveVelocity.y + gravity.y + externalVelocity.y < 0f)
        {
            gravity.y = -2f; //keeps you pressed onto the ground and slopes
        }
        else
        {
            //apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            gravity.y += Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;
            gravity.y = Mathf.Max(gravity.y, -terminalVelocity);
        }
    }

    protected virtual void HandleRotation()
    { 
        if (horizontalVelocity.sqrMagnitude > 0.01f)
        {
            targetYaw = Mathf.Atan2(horizontalVelocity.x, horizontalVelocity.y) * Mathf.Rad2Deg;
        }

        Quaternion targetRotation = Quaternion.Euler(0f, targetYaw, 0f);
        rb.MoveRotation( Quaternion.RotateTowards(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
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
        
        //nearly flat ground dont correct for optimiztion
        if (Vector3.Dot(Vector3.up, groundHit.normal) > 0.999f) return velocity;

        Vector3 hVel = new Vector3(velocity.x, 0f, velocity.z);
        if (hVel.sqrMagnitude < 1e-4f) return Vector3.zero; //standing still prevent sliding
        
        return Vector3.ProjectOnPlane(hVel, groundHit.normal).normalized * hVel.magnitude;
    }

    public void ApplyForce(Vector3 force)
    {
        externalVelocity += force;
    }

    public void OverrideMovement(Vector3 movement)
    {
        Mode = MovementMode.Override;
        moveVelocity = movement;
    }

    public void EndOverrideMovement()
    {
        Mode = MovementMode.Normal;
        StopMovement();
    }

    public void SetMovementMode(MovementMode mode)
    {
        Mode = mode;
    }

    public void EnableMovement()
    {
        Mode = MovementMode.Normal;
        StopMovement();
    }
    
    public void DisableMovement()
    {
        Mode = MovementMode.Disabled;
        StopMovement();
    }

    public void StopMovement()
    {
        moveVelocity = Vector3.zero;
        animator.SetFloat("Speed", 0);
    }
    
    public void Jump()
    {
        if (!IsGrounded || Mode != MovementMode.Normal) return;
        if (abilitySystem.HasTag("Block.Jump")) return;
        moveVelocity.y = Mathf.Sqrt(2f * (Physics.gravity.magnitude * gravityMultiplier) * jumpHeight);
        IsJumping = true;
        animator.CrossFade("Jump", 0.1f);
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
