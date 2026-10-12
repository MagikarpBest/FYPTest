using System;
using UnityEngine;
using UnityEngine.InputSystem;
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


    public enum MovementState
    {
        Walking,
        Jumping,
        Falling,
        
    }
    public MovementMode Mode {get; private set; } = MovementMode.Normal;

    [SerializeField] protected float rotationSmoothTime = 0.1f;
    // [SerializeField] protected float rotationSpeed = 727f;
    [SerializeField] protected float groundCheckDistance = 0.5f;
    [SerializeField] protected float groundCheckPosOffset = -0.14f;
    [SerializeField] protected float groundCheckRadius = 0.28f;
    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected float maxSlopeAngle = 45f;
    [SerializeField] protected float moveSpeed = 11f;
    [SerializeField] protected float gravityMultiplier = 2f;
    [SerializeField] protected float jumpHeight = 3f;
    [SerializeField] protected float externalVelocityDecay = 2f;
    [SerializeField] protected float maxFallSpeed = 53.0f;
    [SerializeField] protected float jumpBufferTime = 0.15f;
    [SerializeField] protected float coyoteTime = 0.15f;
    
    protected Rigidbody rb;
    protected Animator animator;
    protected AbilitySystem abilitySystem;
    
    protected Vector3 moveVelocity = Vector3.zero;
    protected Vector2 horizontalMoveVelocity => new Vector2(moveVelocity.x, moveVelocity.z);
    protected Vector3 externalVelocity = Vector3.zero;
    
    protected bool requestJump;
    protected float jumpBufferTimer = 0f;
    protected float coyoteTimer = 0f;

    protected float targetYaw;
    protected RaycastHit groundHit;
    protected float cosMaxSlopeAngle;
    protected float rotationVelocity;
    private float currentYaw;
    
    public bool IsGrounded { get; private set; } = true;
    bool IsFalling => !IsGrounded && rb.linearVelocity.y <= 0f;
  
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        rb.useGravity = false;
        rb.freezeRotation = true;
        currentYaw = targetYaw = rb.rotation.eulerAngles.y;
        cosMaxSlopeAngle = Mathf.Cos(maxSlopeAngle * Mathf.Deg2Rad);
        abilitySystem = GetComponent<AbilitySystem>();
    }
    
    private void Update()
    {
        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            ApplyForce(new Vector3(20, 20, 20));
        }

        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            ApplyForce(new Vector3(11, 20, 10));
        }
        
        // if (IsWalking)
        // {
        //     abilitySystem.AddTagUnique("Movement.Walking");
        // }
        // else
        // {
        //     abilitySystem.RemoveTag("Movement.Walking");
        // }
        //
        // if (IsGrounded)
        // {
        //     abilitySystem.RemoveTag("State.Airborne");
        //     abilitySystem.AddTagUnique("State.Grounded");
        //     
        //     if (IsJumping && rb.linearVelocity.y < 0f)
        //     { 
        //         moveVelocity.y = 0f; 
        //         IsJumping = false;
        //     }
        // }
        // else
        // {
        //     abilitySystem.RemoveTag("State.Grounded");
        //     abilitySystem.AddTagUnique("State.Airborne");
        // }
        //
        // animator.SetBool("IsFalling", IsFalling);
        
        //bool isFalling !IsGrounded && 
    }
    
    private void FixedUpdate()
    {
        GroundCheck();
        Move();
        HandleRotation();
        HandleJump();
        HandleExternalVelocity();
        HandleGravity();
        ApplyVelocity();
    }

    protected abstract void Move();
    protected void ApplyVelocity()
    {
        Vector3 totalVelocity = SlopeCorrection(moveVelocity) + externalVelocity;
        
        rb.linearVelocity = totalVelocity;
        
        float speed = horizontalMoveVelocity.magnitude / moveSpeed;

        if (speed <= 0f)
        {
            speed = 0f;
        }
        else if (speed <= 0.5f)
        {
            speed = 0.5f;
        }
        else
        {
            speed = 1f;
        }
        animator.SetFloat("Speed", speed);
    }
    
    protected void HandleGravity()
    {
        //stop velocity from dropping infinitely while grounded
        if (IsGrounded && moveVelocity.y < 0)
        {
            moveVelocity.y = -2f;
        }
        else
        {
            //apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            moveVelocity.y += Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;
            moveVelocity.y = Mathf.Max(moveVelocity.y , -maxFallSpeed);
        }
    }

    protected void HandleRotation()
    { 
        if (horizontalMoveVelocity.sqrMagnitude > 1e-4f)
        {
            targetYaw = Mathf.Atan2(horizontalMoveVelocity.x, horizontalMoveVelocity.y) * Mathf.Rad2Deg;
        }

        // Quaternion targetRotation = Quaternion.Euler(0f, targetYaw, 0f);
        // rb.MoveRotation( Quaternion.RotateTowards(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        
        currentYaw = Mathf.SmoothDampAngle
        (
            currentYaw,
            targetYaw,
            ref rotationVelocity,
            rotationSmoothTime,
            Mathf.Infinity,
            Time.fixedDeltaTime
        );
        
        rb.MoveRotation(Quaternion.Euler(0f, currentYaw, 0f));
    }
    
    protected void GroundCheck()
    {
        Vector3 spherePos = transform.position + Vector3.down * groundCheckPosOffset;
        IsGrounded = 
            Physics.SphereCast(spherePos, groundCheckRadius, Vector3.down, out groundHit, groundCheckDistance, groundLayer, QueryTriggerInteraction.Ignore) 
            && Vector3.Dot(Vector3.up, groundHit.normal) >= cosMaxSlopeAngle;
    }
    
    protected void HandleExternalVelocity()
    {
        float dampingFactor = Mathf.Max(0f, 1f - externalVelocityDecay * Time.fixedDeltaTime);
        externalVelocity.x *= dampingFactor;
        externalVelocity.z *= dampingFactor;
    }
    
    protected Vector3 SlopeCorrection(Vector3 vel)
    {
        if (!IsGrounded || vel.y > 0f) return vel; //prevents messing up jump
        
        //nearly flat ground dont correct for optimiztion
        if (Vector3.Dot(Vector3.up, groundHit.normal) > 0.999f) return vel;

        Vector3 horizontalVel = new Vector3(vel.x, 0f, vel.z);
        if (horizontalVel.sqrMagnitude < 1e-4f) return Vector3.zero; //standing still prevent sliding
        
        return Vector3.ProjectOnPlane(horizontalVel, groundHit.normal).normalized * horizontalVel.magnitude;
    }
    
    protected virtual void HandleJump()
    {
        //jump buffer
        if (requestJump)
        {
            jumpBufferTimer = jumpBufferTime;
            requestJump = false;
        }
        else
        {
            jumpBufferTimer -= Time.fixedDeltaTime;
        }
        
        //coyote
        if (rb.linearVelocity.y > 0f)
        {
            coyoteTimer = 0f;
        }
        else if (IsGrounded)
        {
            coyoteTimer = coyoteTime;
        }
        else
        {
            coyoteTimer -= Time.fixedDeltaTime;
        }
        
        //process jump
        if (coyoteTimer > 0f && jumpBufferTimer > 0f)
        {
            moveVelocity.y = Mathf.Sqrt(2f * (Physics.gravity.magnitude * gravityMultiplier) * jumpHeight);
            animator.CrossFade("Jump", 0.1f);
            jumpBufferTimer = 0f;
        }
    }
    
    public void Jump()
    {
        if (Mode != MovementMode.Normal) return;
        if (abilitySystem.HasTag("Block.Jump")) return;
        requestJump = true;
    }
    
    public void ApplyForce(Vector3 force)
    {
        externalVelocity += new Vector3(force.x, 0f, force.z);
        
        if (force.y > 0f)
        { 
            moveVelocity.y = Mathf.Max(moveVelocity.y, force.y); //never weaker than what youre already doing
        }
        else if (force.y < 0f)
        {
            moveVelocity.y = Mathf.Min(moveVelocity.y, force.y); //take the stronger downward
        }
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
