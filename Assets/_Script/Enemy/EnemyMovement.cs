using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour, ILaunchable, IMovementController
{
    [Header("Enemy Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float recoverDelay = 0.5f;

    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private Rigidbody rigidBody;

    public bool IsGrounded { get; private set; } = true;
    public bool IsMoving { get; private set; }
    public bool IsLaunched { get; private set; }
    public bool IsFalling { get; private set; }
    
    private float groundCheckDistance = 0.7f;
    private float launchTimer;

    public void Init(NavMeshAgent agent, Rigidbody rb,Animator animator)
    {
        navMeshAgent = agent;
        rigidBody = rb;
        this.animator = animator;
        navMeshAgent.enabled = true;
    }

    private void Update()
    {
        CheckGround();
        
        if (IsLaunched)
        {
            launchTimer -= Time.deltaTime;
            LaunchCheck();
        }

    }
    
    private void CheckGround()
    {
        IsGrounded = false;

        if (Physics.Raycast(
                transform.position,
                Vector3.down,
                out RaycastHit hit,
                groundCheckDistance + 0.5f,
                groundLayer
            ))
        {
            IsGrounded = true;
        }
    }
    
    public void Move(Vector3 target)
    {
        if (navMeshAgent == null || IsLaunched)
        {
            IsMoving = false;
            return;
        }
        IsMoving = true;
        navMeshAgent.SetDestination(target);
        animator.SetFloat("Speed", 1.0f);

    }
    public void StopMove()
    {
        IsMoving = false;
        navMeshAgent.ResetPath();
        animator.SetFloat("Speed", 0.0f); 
    }

    private void RecoverFromLaunch()
    {
        rigidBody.isKinematic = true;

        navMeshAgent.enabled = true;
        navMeshAgent.Warp(transform.position);
    }

    public void Launch(Vector3 force)
    {
        navMeshAgent.enabled = false;

        rigidBody.isKinematic = false;
        rigidBody.linearVelocity = force;

        IsGrounded = false;
        IsLaunched = true;

        launchTimer = recoverDelay;
    }

    private void LaunchCheck()
    {
        if (!IsLaunched || launchTimer > 0)
        {
            return;
        }

        if (IsGrounded && rigidBody.linearVelocity.y <= 0)
        {
            IsLaunched = false;
            RecoverFromLaunch();
        }
    }
}
