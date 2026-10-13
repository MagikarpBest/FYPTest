using System;
using UnityEngine;

public class PlayerMovement : MovementController
{
    private Camera mainCamera;
    private PlayerInputManager input;
    
    protected override void Awake()
    {
        base.Awake();
        mainCamera = Camera.main;
        input = GetComponent<PlayerInputManager>();
    }

    private void OnEnable()
    {
        input.OnJumpPressed += Jump;
    }

    private void OnDisable()
    {
        input.OnJumpPressed -= Jump;
    }

    protected override void Move()
    {
        if (abilitySystem.HasTag("Block.Movement"))
        {
            StopMovement();
            return;
        }

        Vector3 inputDir = new Vector3(input.MoveInput.x, 0, input.MoveInput.y);
        float cameraYaw = mainCamera.transform.eulerAngles.y;
        moveDir = Quaternion.Euler(0f, cameraYaw, 0f) * inputDir; //rotates inputDir by cameraYaw
        
        float facingDot = Vector3.Dot(transform.forward, moveDir.normalized);
        
        if (horizontalMoveVelocity.sqrMagnitude < 1e-4f && facingDot <= 0.3f) //if not moving and not facing same dir as movedir
        {
            moveVelocity.x = 0f;
            moveVelocity.z = 0f;
        }
        else
        {
            moveVelocity.x = moveDir.x * moveSpeed;
            moveVelocity.z = moveDir.z * moveSpeed;
        }
    }
}
