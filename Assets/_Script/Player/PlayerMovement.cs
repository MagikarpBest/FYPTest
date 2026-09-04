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
    private MovingPlatform currentPlatform;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }



    void Update()
    {
        Debug.Log(controller.isGrounded);

        //CheckPlatform();
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
        Vector3 finalMovement = Vector3.zero;
        //if player standing on moving platform
        if(currentPlatform!=null)
        {
            Debug.Log("Delta: " + currentPlatform.DeltaMovement);

            finalMovement += currentPlatform.DeltaMovement;
        }
        
        // player movement
        Vector3 velocity = moveDirection * moveSpeed;
        velocity.y = verticalVelocity;

        finalMovement += velocity * Time.deltaTime;
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
        if (controller.isGrounded||currentPlatform!=null)
        {
            Debug.Log("Jump");
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
    
    private void ApplyGravity()
    {
        //Debug.Log("Platform: " + currentPlatform);
        Debug.Log($"{controller.isGrounded}");
        // Gravity
        if(currentPlatform != null)
        {
            verticalVelocity = 0f;
            return;
        }
        if(controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }
        else
        {
            
            verticalVelocity += gravity * Time.deltaTime;
        }
    }
    
    private void CheckPlatform()
    {
        currentPlatform = null;
        if(!controller.isGrounded)
        {
            return;
        }
        
        // shoot a ray from character feet to platform
        Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);
        
        if (Physics.Raycast(ray,out RaycastHit hit,0.3f))
        {
            currentPlatform = hit.collider.GetComponent<MovingPlatform>();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        MovingPlatform pillar = other.GetComponentInParent<MovingPlatform>();
        if(pillar != null)
        {
            currentPlatform = pillar;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        MovingPlatform pillar = other.GetComponentInParent<MovingPlatform>();

        if(pillar != null && currentPlatform == pillar)
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
