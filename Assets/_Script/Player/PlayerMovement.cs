using System;
using UnityEditor.Overlays;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, ILaunchable
{
    [Header("Reference")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private PlayerInputManager inputManager;

    [Header("Movement settings")]
    private float moveSpeed = 9f;
    private float rotationSpeed = 10f;
    [SerializeField] private float gravity = -9.81f * 3;
    [SerializeField] private float jumpHeight = 1f;

    private Transform cameraTransform;
    private float verticalVelocity;
    private MovingPlatform currentPlatform;

    private bool isJumping;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }


    void Update()
    {
        Debug.Log(controller.isGrounded);

        ApplyGravity();
        HandleMove();

    }


    private void HandleMove()
    {
        Vector2 moveInput = inputManager.MoveInput;
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = forward * moveInput.y + right * moveInput.x;

        // Moving platform movement 
        Vector3 finalMovement = Vector3.zero;
        // if player standing on moving platform
        if (currentPlatform != null)
        {
            Debug.Log("Delta: " + currentPlatform.DeltaMovement);

            finalMovement += currentPlatform.DeltaMovement;
        }

        // Player directional movement
        Vector3 velocity = moveDirection * moveSpeed;
        velocity.y = verticalVelocity;

        finalMovement += velocity * Time.deltaTime;
        ResolveMovingPlatformCollision(ref finalMovement);
        controller.Move(finalMovement);

        // Rotate towards movement direction
        if (moveInput.sqrMagnitude > 0.01f)
        {
            Quaternion target = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, rotationSpeed * Time.deltaTime);
        }

    }

    private void HandleJump()
    {
        // Jump
        if (controller.isGrounded || currentPlatform != null)
        {
            Debug.Log("Jump");
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            isJumping = true;
        }
    }

    private void ApplyGravity()
    {
        //Debug.Log("Platform: " + currentPlatform);
        Debug.Log($"{controller.isGrounded}");
        // Gravity
        // if(currentPlatform != null&&!isJumping)
        // {
        //     verticalVelocity = 0f;
        //     return;
        // }
        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
            isJumping = false;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
    }

    public void Launch(Vector3 force)
    {
        verticalVelocity = force.y+force.z;
        currentPlatform = null;
        isJumping = true;
    }

    // faking collision
    private void ResolveMovingPlatformCollision(ref Vector3 movement)
    {
        if (currentPlatform == null) return;

        Collider pillarCollider = currentPlatform.PlatformCollider;
        Vector3 direction;
        float distance;

        // Check if the CharacterController is overlapping with the pillars trigger collider
        //https://docs.unity3d.com/6000.5/Documentation/ScriptReference/Physics.ComputePenetration.html
        bool overlap = Physics.ComputePenetration(
            controller,
            transform.position + movement, // predict where player be after movement
            transform.rotation,
            pillarCollider,
            pillarCollider.transform.position,
            pillarCollider.transform.rotation,
            out direction,
            out distance
        );

        if (overlap)
        {
            // If the push direction is upwards, the player is on top of the pillar
            if (Vector3.Dot(direction, Vector3.up) > 0.5f)
            {
                // Apply separation to stay on top
                movement += direction * distance;

                // Reset gravity and jumping state as we have "landed"
                if (verticalVelocity < 0)
                {
                    verticalVelocity = -2f; // Small downward force to stay grounded
                    isJumping = false;
                }
            }
            else
            {
                // For side collisions, just remove the movement into the wall
                float pushAmount = Vector3.Dot(movement, direction);
                if (pushAmount < 0)
                {
                    movement -= direction * pushAmount;
                }
            }
        }
    }

    // Moving platform velocity code
    private void OnTriggerStay(Collider other)
    {
        MovingPlatform pillar = other.GetComponentInParent<MovingPlatform>();
        if (pillar != null)
        {
            currentPlatform = pillar;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        MovingPlatform pillar = other.GetComponentInParent<MovingPlatform>();

        if (pillar != null && currentPlatform == pillar)
        {
            currentPlatform = null;
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
