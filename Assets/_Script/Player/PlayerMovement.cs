using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private PlayerInputManager inputManager;
    
    [Header("Movement settings")]
    private float moveSpeed = 7f;
    private float rotationSpeed = 10f;
    [SerializeField]private float gravity = -9.81f*3;
    [SerializeField]private float jumpHeight = 1f;

    
    private Transform cameraTransform;
    private float verticalVelocity;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }



    void Update()
    {
        HandleMove();
        ApplyGravity();
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
        
        // Final movement
        Vector3 velocity = moveDirection * moveSpeed;
        velocity.y = verticalVelocity;
        
        controller.Move(velocity * Time.deltaTime);
        
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
        if (controller.isGrounded)
        {
            Debug.Log("Jump");
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
    
    private void ApplyGravity()
    {
        // Gravity
        if (controller.isGrounded)
        {
            if (verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
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
