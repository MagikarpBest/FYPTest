using UnityEngine;

public class PlayerMovementRB : MonoBehaviour, ILaunchable
{
    [Header("Reference")]
    [SerializeField] private Rigidbody rigidBody;

    [SerializeField] private Transform model;
    [SerializeField] private float rotationSpeed = 720f;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;


    [Header("Movement settings")]
    [SerializeField] private float gravityMultiply = 2;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float moveSpeed = 11f;
    [SerializeField] private float externalDecaySpeed = 2f;

    // for direction movement
    private Transform cameraTransform;
    private bool isGrounded;
    private bool isLaunched;
    private Vector3 currentNormal;
    private Vector3 currentMoveDirection;
    private Vector3 moveVelocity;
    private Vector3 externalVelocity; // knockback XZ

    private Vector3 groundNormal;

    private PlayerInputManager inputManager;

    [SerializeField] private Animator animator;

    private void Update()
    {
        HandleRotation();
    }

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
        inputManager = FindFirstObjectByType<PlayerInputManager>();
    }

    private void FixedUpdate()
    {
        LaunchCheck();
        CheckGround();
        ApplyExternalVelocity();
        HandleMovement();
        HandleGravity();
        ApplyVelocity();
        currentNormal = Vector3.zero;

        
    }
    private void LaunchCheck()
    {
        if(!isLaunched)
        {
            return;
        }
        
        if(isGrounded&&rigidBody.linearVelocity.y<=0)
        {
            isLaunched = false;
        }
    }

    private void CheckGround()
    {
        isGrounded = false;

        if (Physics.Raycast(
                transform.position,
                Vector3.down,
                out RaycastHit hit,
                groundCheckDistance + 0.5f,
                groundLayer
            ))
        {
            isGrounded = true;
            groundNormal = hit.normal;
        }
    }

    // decay
    private void ApplyExternalVelocity()
    {
        // decay only horizontal velocity
        Vector3 horizontal = new Vector3(externalVelocity.x, 0, externalVelocity.z);

        horizontal = Vector3.Lerp(horizontal, Vector3.zero, externalDecaySpeed * Time.fixedDeltaTime);

        externalVelocity.x = horizontal.x;
        externalVelocity.z = horizontal.z;
    }
    
    
    private void HandleMovement()
    {
        
        if(isLaunched)
        {
            moveVelocity = Vector3.zero;
            return;
        }
        Vector2 moveInput = inputManager.MoveInput;

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
        // remove movement into wall
        if (currentNormal != Vector3.zero)
        {
            direction = Vector3.ProjectOnPlane(direction, currentNormal);
        }

        moveVelocity = direction * moveSpeed;
        animator.SetFloat("Speed",direction.magnitude);
    }

    private void HandleJump()
    {
        if (!isGrounded)
        {
            return;
        }

        Vector3 velocity = rigidBody.linearVelocity;
        velocity.y = Mathf.Sqrt(2f * (Physics.gravity.magnitude * gravityMultiply) * jumpHeight);
        rigidBody.linearVelocity = velocity;
    }

    // Visual rotation when moving
    private void HandleRotation()
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

    private void HandleGravity()
    {
        //externalVelocity.y += gravity;
        rigidBody.AddForce(Physics.gravity * gravityMultiply, ForceMode.Acceleration);
        //Debug.Log($"linear velocity ={rigidBody.linearVelocity}");
    }

    // External force
    public void Launch(Vector3 force)
    {
        //externalVelocity += force;
        isGrounded = false;
        isLaunched = true;
        externalVelocity += new Vector3(force.x, 0, force.z);

        rigidBody.linearVelocity = new Vector3(rigidBody.linearVelocity.x, force.y, rigidBody.linearVelocity.z);
    }

    private void ApplyVelocity()
    {
        Vector3 velocity = rigidBody.linearVelocity;
        Vector3 targetHorizontalVelocity = moveVelocity + externalVelocity;
        

        if (currentNormal != Vector3.zero)
        {
            // Project our target movement so it doesn't push into the wall
            targetHorizontalVelocity = Vector3.ProjectOnPlane(targetHorizontalVelocity, currentNormal);
        
            // If the current physics velocity is already pushing AWAY from the wall,
            // keep that extra push-out velocity.
            float currentPushOut = Vector3.Dot(velocity, currentNormal);
            if (currentPushOut > 0)
            {
                targetHorizontalVelocity += currentNormal * currentPushOut;
            }
        }

        velocity.x = targetHorizontalVelocity.x;
        velocity.z = targetHorizontalVelocity.z;


        rigidBody.linearVelocity = velocity;
        
        
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
                
                currentNormal = contact.normal;
            
                // If the physics engine says we are overlapping (separation < 0), 
                if (contact.separation < 0)
                {
                    rigidBody.position += contact.normal * -contact.separation;
                }
            }
        }
    }
    

    private void OnEnable()
    {
        inputManager.OnJumpPressed += HandleJump;
    }


    private void OnDisable()
    {
        inputManager.OnJumpPressed -= HandleJump;
    }
}
