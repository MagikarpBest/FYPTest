using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, ILaunchable
{
    private NavMeshAgent agent;
    [SerializeField] private Transform player;
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float recoverDelay = 0.5f;


    private bool isGrounded = true;
    private bool isLaunched;
    private float groundCheckDistance = 0.7f;
    private Vector3 groundNormal;
    private float launchTimer;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        // agent.updatePosition = false;
        // agent.updateRotation = false;
        agent.enabled = true;
    }


    private void Update()
    {
        CheckGround();

        if (isLaunched)
        {
            launchTimer -= Time.deltaTime;
            LaunchCheck();
        }
        HandleMovement();

        //Debug.Log(launchTimer);
    }

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
            groundNormal = hit.normal;
        }
    }

    private void HandleMovement()
    {
        if (agent.enabled && !isLaunched )
        {
            agent.SetDestination(player.position);
        }
    }

    private void RecoverFromLaunch()
    {
        rigidBody.isKinematic = true;

        agent.enabled = true;
        agent.Warp(transform.position);
    }

    public void Launch(Vector3 force)
    {
        agent.enabled = false;

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
