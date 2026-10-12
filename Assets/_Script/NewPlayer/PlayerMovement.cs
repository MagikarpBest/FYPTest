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
        if (Mode != MovementMode.Normal) return;
        if (abilitySystem.HasTag("Block.Movement"))
        {
            StopMovement();
            return;
        }

        Vector3 inputDir = new Vector3(input.MoveInput.x, 0, input.MoveInput.y);
        float cameraYaw = mainCamera.transform.eulerAngles.y;
        Vector3 moveDir = Quaternion.Euler(0f, cameraYaw, 0f) * inputDir; //rotates inputDir by cameraYaw
        
        moveVelocity.x = moveDir.x * moveSpeed;
        moveVelocity.z = moveDir.z * moveSpeed;
    }
}
