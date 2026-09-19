using UnityEngine;

public class PlayerMovementRB : MonoBehaviour, ILaunchable
{
    [Header("Reference")]
    [SerializeField] private Rigidbody rb;

    [SerializeField] private Transform model;
    [SerializeField] private float rotationSpeed = 720f;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;


    [Header("Movement settings")]
    [SerializeField] private float gravity = 9.81f * 2;
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float moveSpeed = 9f;
    [SerializeField] private float externalDecay = 5f;

    // for direction movement
    private Transform cameraTransform;
    private bool grounded;
    private Vector3 currentNormal;
    private Vector3 currentMoveDirection;
    private Vector3 moveVelocity;
    private Vector3 externalVelocity;


    private Vector3 groundNormal;

    private PlayerInputManager inputManager;

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
        CheckGround();
        ApplyExternalVelocity();
        HandleMovement();
        //HandleGravity();
        ApplyVelocity();
    }

    private void CheckGround()
    {
        grounded = false;

        if (Physics.Raycast(
                transform.position,
                Vector3.down,
                out RaycastHit hit,
                groundCheckDistance + 0.5f,
                groundLayer
            ))
        {
            grounded = true;
            groundNormal = hit.normal;
        }
    }
    
    // decay
    private void ApplyExternalVelocity()
    {
        // decay only horizontal velocity
        Vector3 horizontal = new Vector3(externalVelocity.x, 0, externalVelocity.z);

        horizontal = Vector3.Lerp(horizontal, Vector3.zero, externalDecay * Time.fixedDeltaTime);

        externalVelocity.x = horizontal.x;
        externalVelocity.z = horizontal.z;
    }
    
    
    private void HandleMovement()
    {
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

        currentNormal = Vector3.zero;
    }

    private void HandleJump()
    {
        if (!grounded)
        {
            return;
        }

        Vector3 velocity = rb.linearVelocity;
        velocity.y = Mathf.Sqrt(2f * gravity * jumpHeight);
        rb.linearVelocity = velocity;
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
        rb.AddForce(Physics.gravity * gravity, ForceMode.Acceleration);
    }
    
    // External force
    public void Launch(Vector3 force)
    {
        externalVelocity += force;
        grounded = false;
    }
    
    private void ApplyVelocity()
    {
        Vector3 finalVelocity = moveVelocity + externalVelocity;

        finalVelocity.y = rb.linearVelocity.y + externalVelocity.y;
        rb.linearVelocity = finalVelocity;
        externalVelocity.y = 0;
    }

    // Ground check
    private void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.5f)
            {
                grounded = true;
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
        grounded = false;
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
