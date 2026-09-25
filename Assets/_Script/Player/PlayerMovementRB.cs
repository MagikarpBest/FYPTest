using System;
using UnityEngine;

public class PlayerMovementRB : MonoBehaviour, ILaunchable
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

    // for direction movement
    private Transform cameraTransform;
    private bool isGrounded;
    private bool isLaunched;
    
    private Vector3 currentMoveDirection;
    private Vector3 moveVelocity;
    private Vector3 externalVelocity; // knockback XZ

    // state machine test
    public bool IsGrounded => isGrounded;
    public bool IsLaunched => isLaunched;
    
    public Action OnLaunched;   
    private Animator animator;

    // private void Start()
    // {
    //     //Application.targetFrameRate = 60;
    // }
    // private void Awake()
    // {
    //     cameraTransform = Camera.main.transform;
    //     animator = GetComponent<Animator>();
    // }

    public void Init(Rigidbody rigidBody, Transform model, Transform cameraTransform, Animator animator)
    {
        this.rigidBody = rigidBody;
        this.model = model;
        this.cameraTransform = cameraTransform;
        this.animator = animator;
    }

    //These should handle in your states
    private void Update()
    {
        LaunchCheck();
        CheckGround();
        ApplyExternalVelocity();

    }
    
    
    private void FixedUpdate()
    {
        HandleGravity();
        ApplyVelocity();
    }
    
    // ==========================
    // Ground
    // ==========================
    public void LaunchCheck()
    {
        if(!isLaunched)
        {
            return;
        }
        
        if(isGrounded&&rigidBody.linearVelocity.y<=0)
        {
            isLaunched = false;
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
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
        //Debug.Log(isGrounded);
    }

    // ==========================
    // Physics
    // ==========================
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

    
    // ==========================
    // Movement
    // ==========================
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

    // MOVE TO ANIMATION
    // Visual rotation when moving
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
    
    // ==========================
    // Launch
    // ==========================
    // External force
    public void Launch(Vector3 force)
    {
        //externalVelocity += force;
        isGrounded = false;
        isLaunched = true;
        externalVelocity += new Vector3(force.x, 0, force.z);

        rigidBody.linearVelocity = new Vector3(rigidBody.linearVelocity.x, force.y, rigidBody.linearVelocity.z);
        OnLaunched?.Invoke();
    }

    // Ground check
    private void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            float dot = Vector3.Dot(contact.normal, Vector3.up);

            if (dot > 0.5f)
            {
                isGrounded = true;
            }
            else
            {

            }
        }
    }

}
