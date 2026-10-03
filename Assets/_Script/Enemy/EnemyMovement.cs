using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour, ILaunchable
{
    [Header("Enemy Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float recoverDelay = 0.5f;

    private Animator animator;
    private NavMeshAgent navMeshAgent;


    // movement
    private Rigidbody rigidBody;
    private bool isGrounded = true;
    private bool isMoving;
    private bool isLaunched;
    private float groundCheckDistance = 0.7f;
    private float launchTimer;

  

    public bool IsGrounded => isGrounded;
    public bool IsMoving => isMoving;



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
        
        if (isLaunched)
        {
            launchTimer -= Time.deltaTime;
            LaunchCheck();
        }

    }

    // GENERAL STUFF
    private void CheckGround()
    {
        isGrounded = false;

        if (Physics.Raycast(
                transform.position,
                Vector3.down,
                out RaycastHit hit,
                groundCheckDistance + 0.5f,
                groundLayer
            ))
        {
            isGrounded = true;
        }
    }
    
    
    // MOVEMENT
    public void Move(Vector3 target)
    {
        if (navMeshAgent == null || isLaunched)
        {
            isMoving = false;
            return;
        }
        isMoving = true;
        navMeshAgent.SetDestination(target);
        animator.SetFloat("Speed", 1.0f);

    }
    public void StopMove()
    {
        isMoving = false;
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

        isGrounded = false;
        isLaunched = true;

        launchTimer = recoverDelay;
    }

    private void LaunchCheck()
    {
        if (!isLaunched || launchTimer > 0)
        {
            return;
        }

        if (isGrounded && rigidBody.linearVelocity.y <= 0)
        {
            isLaunched = false;
            RecoverFromLaunch();
        }
    }
}
