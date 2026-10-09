// using System;
// using UnityEngine;
//
// public class PlayerMovement : MonoBehaviour, IMovementController
// {
//     [SerializeField] private float rotationSmoothTime = 720f;
//     [SerializeField] private float groundCheckDistance = 0.2f;
//     [SerializeField] private LayerMask groundLayer;
//     [SerializeField] private float moveSpeed = 11f;
//     [SerializeField] private float gravityMultiplier = 2;
//     [SerializeField] private float jumpHeight = 3f;
//     [SerializeField] private float externalVelocityDecay = 2f;
//     [SerializeField] private float terminalVelocity;
//     
//     private Rigidbody rb;
//     private Camera mainCamera;
//     private Animator animator;
//     private PlayerInputManager input;
//     
//     private Vector3 lookRotation = Vector3.zero;
//     private float rotationVelocity = 0f;
//     private Vector3 velocity =  Vector3.zero;
//     private Vector3 externalVelocity = Vector3.zero;
//     private bool isGrounded;
//     
//     public void Init(PlayerInputManager input, Camera mainCamera, Animator animator, Rigidbody rb)
//     {
//         this.input = input; 
//         this.mainCamera = mainCamera;
//         this.animator = animator;
//         this.rb = rb;
//     }
//
//     private void FixedUpdate()
//     {
//         rb.linearVelocity = velocity + externalVelocity;    
//     }
//
//     public void Move()
//     {
//         Vector3 targetDirection = Quaternion.Euler(lookRotation) * Vector3.forward;
//         targetDirection = targetDirection.normalized;
//         
//         velocity.x = targetDirection.x * moveSpeed;
//         velocity.z = targetDirection.z * moveSpeed;
//         
//         animator.SetFloat("Speed", 0.5f);
//     }
//
//     public void HandleGravity()
//     {
//         //stop velocity from dropping infinitely while grounded
//         if (isGrounded && velocity.y < 0f)
//         {
//             velocity.y = -2f; // keeps you pressed onto the ground and slopes
//         }
//         else
//         {
//             //apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
//             velocity.y += Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;
//             velocity.y = Mathf.Max(velocity.y, -terminalVelocity);
//         }
//     }
//
//     public void HandleRotation()
//     {
//         //normalize input direction unity input system alr does it but whatever
//         Vector3 inputDirection = new Vector3(input.MoveInput.x, 0.0f, input.MoveInput.y).normalized;
//
//         if (inputDirection != Vector3.zero)
//         {
//             //change input direction to angle then offset by the camera y rot 
//             float targetYaw = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
//             lookRotation.y = targetYaw;
//         }
//
//         float smoothedYaw = Mathf.SmoothDampAngle(transform.eulerAngles.y, lookRotation.y, ref rotationVelocity, rotationSmoothTime);
//
//         //rotate to face input direction relative to camera position
//         rb.MoveRotation(Quaternion.Euler(0.0f, smoothedYaw, 0.0f));
//     }
//     
//     private void GroundedCheck()
//     {
//         Vector3 spherePos = transform.position + Vector3.down * GroundedOffset;
//
//         IsGrounded = Physics.SphereCast(spherePos, GroundedRadius, Vector3.down, out groundHit, GroundCheckDistance, GroundLayers, QueryTriggerInteraction.Ignore);
//     }
//     
//    
// }
