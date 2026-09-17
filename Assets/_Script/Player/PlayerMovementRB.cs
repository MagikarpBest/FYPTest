using UnityEngine;

public class PlayerMovementRB : MonoBehaviour, ILaunchable
{
    [Header("Reference")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private PlayerInputManager inputManager;
    [SerializeField] private Transform model;
    [SerializeField] private float rotationSpeed = 720f;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;


    [Header("Movement settings")]
    [SerializeField] private float gravity = 9.81f * 2;
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float moveSpeed = 9f;

    // for direction movement
    private Transform cameraTransform;
    private bool grounded;
    private Vector3 currentNormal;
    private Vector3 currentMoveDirection;

    private Vector3 groundNormal;

    private MovingPlatform currentPlatform;


    private void Update()
    {
        HandleRotation();
    }

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }


    private void FixedUpdate()
    {
        CheckGround();
        HandleMovement();
        HandleGravity();
        // StickToGround();

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

    private void StickToGround()
    {
        if (!grounded)
        {
            return;
        }

        if (rb.linearVelocity.y <= 0)
        {
            rb.AddForce(
                -groundNormal * 30f,
                ForceMode.Acceleration
            );
        }
    }

    private void HandleRotation()
    {
        if (currentMoveDirection.sqrMagnitude < 0.01f)
        {
            return;
        }


        Quaternion target =
            Quaternion.LookRotation(currentMoveDirection);


        model.rotation =
            Quaternion.RotateTowards(
                model.rotation,
                target,
                rotationSpeed * Time.deltaTime
            );
    }

    private void HandleGravity()
    {
        rb.AddForce(Physics.gravity * gravity, ForceMode.Acceleration);
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

        Vector3 velocity = rb.linearVelocity;


 


        velocity.x = direction.x * moveSpeed;
        velocity.z = direction.z * moveSpeed ;


        rb.linearVelocity = velocity;


        currentNormal = Vector3.zero;

    }

    private void HandleJump()
    {
        if (!grounded)
        {
            return;
        }

        Debug.Log("Jump");
        rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
    }

    public void Launch(Vector3 force)
    {
        rb.AddForce(
            force,
            ForceMode.Impulse
        );

        grounded = false;
    }

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

    // private void SnapToGround()
    // {
    //     if (currentPlatform != null) return;
    //
    //     if (controller.isGrounded && !isJumping)
    //     {
    //         // Raycast down to find the slope
    //         if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, controller.height / 2f + 0.5f))
    //         {
    //             // Only snap if we aren't already touching the ground (avoid double-move)
    //             if (hit.distance > (controller.height / 2f) + 0.05f)
    //             {
    //                 float snapDistance = hit.distance - (controller.height / 2f);
    //                 controller.Move(new Vector3(0, -snapDistance, 0));
    //             }
    //         }
    //     }
    // }
}