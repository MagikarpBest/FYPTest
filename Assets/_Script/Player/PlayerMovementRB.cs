using System;
using UnityEngine;

public class PlayerMovementRB : MonoBehaviour, ILaunchable, IMovementController
{
    [Header("Reference")]
    private Rigidbody rigidBody;
    private Transform model;
    [SerializeField] private float rotationSpeed = 720f;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    
    [Header("Movement ")]
    [SerializeField] private float moveSpeed = 11f;

    [Header("Jump")]
    [SerializeField] private float gravityMultiply = 2;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float externalDecaySpeed = 2f;
    
    private Transform cameraTransform;
    public bool IsGrounded { get; private set; } = true;
    public bool IsMoving { get; private set; }
    public bool IsLaunched { get; private set; }
    public bool IsFalling { get; private set; }
    
    private Vector3 currentMoveDirection;
    private Vector3 moveVelocity;
    private Vector3 externalVelocity; // knockback XZ
    
    public Action OnLaunched;   
    private Animator animator;
    
    public void Init(Rigidbody rigidBody, Transform model, Transform cameraTransform, Animator animator)
    {
        this.rigidBody = rigidBody;
        this.model = model;
        this.cameraTransform = cameraTransform;
        this.animator = animator;
    }
    
    private void Update()
    {
        LaunchCheck();
        CheckGround();
        ApplyExternalVelocity();
        //Debug.Log(moveVelocity);

    }
    
    private void FixedUpdate()
    {
        HandleGravity();
        ApplyVelocity();
    }
    
    public void LaunchCheck()
    {
        if(!IsLaunched)
        {
            return;
        }
        
        if(IsGrounded&&rigidBody.linearVelocity.y<=0)
        {
            IsLaunched = false;
        }
        //Debug.Log(isLaunched);
    }

    public void CheckGround()
    {
        if (Physics.Raycast(
                rigidBody.position,
                Vector3.down,
                out RaycastHit hit,
                groundCheckDistance + 0.5f,
                groundLayer
            ))
        {
            IsGrounded = true;
        }
        else
        {
            IsGrounded = false;
        }
        //Debug.Log(isGrounded);
    }

    public void HandleGravity()
    {
        rigidBody.AddForce(Physics.gravity * gravityMultiply, ForceMode.Acceleration);
    }
    // decay
    public void ApplyExternalVelocity()
    {
        // decay only horizontal velocity
        Vector3 horizontal = new Vector3(externalVelocity.x, 0, externalVelocity.z);

        horizontal = Vector3.Lerp(horizontal, Vector3.zero, externalDecaySpeed * Time.deltaTime);

        externalVelocity.x = horizontal.x;
        externalVelocity.z = horizontal.z;
    }
    public void ApplyVelocity()
    {
        Vector3 velocity = rigidBody.linearVelocity;
        Vector3 targetHorizontalVelocity = moveVelocity + externalVelocity;
        
        velocity.x = targetHorizontalVelocity.x;
        velocity.z = targetHorizontalVelocity.z;
        
        rigidBody.linearVelocity = velocity;
        
    }
    
    public void Move(Vector2 moveInput)
    {
        Vector3 forward = cameraTransform.transform.forward;
        Vector3 right = cameraTransform.transform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        // move direction based on camera
        Vector3 direction =
            forward * moveInput.y +
            right * moveInput.x;

        currentMoveDirection = direction;

        moveVelocity = direction * moveSpeed;
        animator.SetFloat("Speed",direction.magnitude);
    }
    
    public void StopMove()
    {
        moveVelocity = Vector3.zero;
        animator.SetFloat("Speed", 0);
    }

    public void Jump()
    {
        Vector3 velocity = rigidBody.linearVelocity;
        velocity.y = Mathf.Sqrt(2f * (Physics.gravity.magnitude * gravityMultiply) * jumpHeight);
        rigidBody.linearVelocity = velocity;
    }
    
    public void RotateTowardsMovement()
    {
        if (currentMoveDirection.sqrMagnitude < 0.01f)
        {
            return;
        }

        Quaternion target = Quaternion.LookRotation(currentMoveDirection);

        model.rotation =
            Quaternion.RotateTowards(
                model.rotation,
                target,
                rotationSpeed * Time.deltaTime
            );
    }
    
    public void Launch(Vector3 force)
    {
        //externalVelocity += force;
        IsGrounded = false;
        IsLaunched = true;
        externalVelocity += new Vector3(force.x, 0, force.z);

        rigidBody.linearVelocity = new Vector3(rigidBody.linearVelocity.x, force.y, rigidBody.linearVelocity.z);
        OnLaunched?.Invoke();
    }
    
    private void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            float dot = Vector3.Dot(contact.normal, Vector3.up);

            if (dot > 0.5f)
            {
                IsGrounded = true;
            }
            else
            {

            }
        }
    }

}
