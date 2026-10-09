using System;
using UnityEngine;

public class PlayerMovement : MovementController
{
    private Camera mainCamera;
    private PlayerInputManager input;
    
    protected override void Start()
    {
        base.Start();
        mainCamera = Camera.main;
        input = GetComponent<PlayerInputManager>();
        input.OnJumpPressed += Jump;
    }

    private void OnDisable()
    {
        input.OnJumpPressed -= Jump;
    }

    private void FixedUpdate()
    {
        HandleGravity();
        Move();
        HandleExternalVelocity();
        ApplyVelocity();
    }

    private void Move()
    {
        if (Mode != MovementMode.Normal) return;

        Vector3 inputDir = new Vector3(input.MoveInput.x, 0, input.MoveInput.y);
        float cameraYaw = mainCamera.transform.eulerAngles.y;
        moveDir = Quaternion.Euler(0f, cameraYaw, 0f) * inputDir; //rotates inputDir by cameraYaw
        
        targetVelocity.x = moveDir.x * moveSpeed;
        targetVelocity.z = moveDir.z * moveSpeed;
    }
}
