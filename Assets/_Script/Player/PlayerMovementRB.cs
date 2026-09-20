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
        currentNormal = Vector3.zero;
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
        Debug.Log($"linear velocity ={rigidBody.linearVelocity}");
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
        
        // Only control horizontal movement
        Vector3 velocity = rigidBody.linearVelocity;
        velocity.x =
            moveVelocity.x +
            externalVelocity.x;


        velocity.z =
            moveVelocity.z +
            externalVelocity.z;


        rigidBody.linearVelocity = velocity;
    }

    // Ground check
    private void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.5f)
            {
                isGrounded = true;
                return;
            }
            else
            {
                currentNormal = contact.normal;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
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
