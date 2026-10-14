using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Threading;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public abstract class MovementController : MonoBehaviour
{
    public enum MovementMode
    {
        Walking,
        Falling,
        Override
    }
    
    public MovementMode CurrentMode { get; private set; } = MovementMode.Walking;
    public MovementMode PreviousMode { get; private set; } = MovementMode.Walking;
    
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
    protected Vector3 externalVelocity = Vector3.zero;
    protected Vector2 horizontalMoveVelocity => new Vector2(moveVelocity.x, moveVelocity.z);
    protected Vector3 moveDir;
    
    protected bool requestJump;
    protected float jumpBufferTimer = 0f;
    protected float coyoteTimer = 0f;

    protected float targetYaw;
    protected RaycastHit groundHit;
    protected float cosMaxSlopeAngle;
    protected float rotationVelocity;
    private float currentYaw;
    
    private CancellationTokenSource overrideCts;
    
    private bool wasGrounded;
    public bool IsGrounded { get; private set; } = true;
    
    private void OnValidate()
    {
        cosMaxSlopeAngle = Mathf.Cos(maxSlopeAngle * Mathf.Deg2Rad);
    }
    
    protected virtual void Awake()
    {
        Application.targetFrameRate = 30;
        
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        rb.useGravity = false;
        rb.freezeRotation = true;
        cosMaxSlopeAngle = Mathf.Cos(maxSlopeAngle * Mathf.Deg2Rad);
        abilitySystem = GetComponent<AbilitySystem>();
        currentYaw = targetYaw = rb.rotation.eulerAngles.y;
    }

    private void Start()
    {
        Application.targetFrameRate = 30;
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
        
        if (Keyboard.current.fKey.wasPressedThisFrame)
        { 
            OverrideMovement(new Vector3(10, 20, 10), 3f);
        }
        
        
        HandleRotation();

        Debug.Log(moveVelocity.y);
    }
    
    private void FixedUpdate()
    {
        GroundCheck();
        
        switch (CurrentMode)
        {
            case MovementMode.Walking:
                HandleWalkingState();
                break;
            case MovementMode.Falling:
                HandleFallingState();
                break;
            //Swimming?
            //Gliding?
            //Crouch?
        }
        
        //HandleRotation();
        HandleJump(); 
        HandleExternalVelocity();
        HandleGravity();
        ApplyVelocity(); 
    }
    
    private void HandleWalkingState()
    {
        if (abilitySystem.HasTag("Block.Movement"))
        {
            StopMovement();
            return;
        }
        Move();
        if (!IsGrounded) SetMovementMode(MovementMode.Falling);
    }

    private void HandleFallingState()
    {
        if (abilitySystem.HasTag("Block.Movement"))
        {
            StopMovement();
            return;
        }
        Move();
        
        //!IsGrounded && rb.linearVelocity.y <= 0f;
        if (IsGrounded) SetMovementMode(MovementMode.Walking);
    }
    
    protected virtual void OnMovementModeChanged(MovementMode prev, MovementMode next)
    {
        //exit
        switch (prev)
        {
        }
        
        //enter
        switch (next)
        {
        }
    }
    
    public void SetMovementMode(MovementMode mode)
    {
        if (mode == CurrentMode) return;
        PreviousMode = CurrentMode;
        CurrentMode = mode;
        OnMovementModeChanged(PreviousMode, mode);
    }
    
    protected abstract void Move();
    
    protected void ApplyVelocity()
    {
        Vector3 totalVelocity = SlopeCorrection(moveVelocity) + externalVelocity;
        
        rb.linearVelocity = totalVelocity;

        if (CurrentMode == MovementMode.Override) return;
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
        if (moveDir.sqrMagnitude > 1e-4f)
        {
            targetYaw = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
        }

        // Quaternion targetRotation = Quaternion.Euler(0f, targetYaw, 0f);
        // rb.MoveRotation( Quaternion.RotateTowards(rb.rotation, targetRotation, 800f * Time.fixedDeltaTime));
        
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

        //update tag
        if (IsGrounded == wasGrounded) return;
        wasGrounded = IsGrounded;
        
        if (IsGrounded)
        {
            abilitySystem.AddTagUnique("State.Grounded");
            abilitySystem.RemoveTag("State.Airborne");
        }
        else
        {
            abilitySystem.AddTagUnique("State.Airborne");
            abilitySystem.RemoveTag("State.Grounded");
        }
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
    
    protected void HandleJump()
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
        if (moveVelocity.y > 0f)
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
        if (abilitySystem.HasTag("Block.Movement")) return;
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
    
    public async void OverrideMovement(Vector3 movement, float duration = -1)
    {
        overrideCts?.Cancel(); //cancel previous if have
        
        var cts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
        overrideCts = cts;

        moveDir = new Vector3(movement.x, 0f, movement.z).normalized;
        moveVelocity = movement;
        SetMovementMode(MovementMode.Override);

        if (duration <= 0f) return; //indefinite until ended manually
        
        try
        {
            await Awaitable.WaitForSecondsAsync(duration, cts.Token);
            SetMovementMode(MovementMode.Walking);
            StopMovement();
        }
        catch (OperationCanceledException)
        {
            
        }
        finally
        {
            if (overrideCts == cts) overrideCts = null; //if only one running
            cts?.Dispose();
        }
    }

    public void EndOverrideMovement()
    {
        overrideCts?.Cancel();    
        SetMovementMode(MovementMode.Walking);
        StopMovement();
    }
    
    public void StopMovement()
    {
        moveDir = Vector3.zero;
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
