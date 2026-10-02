using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, ILaunchable
{
    [Header("Enemy Settings")]
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float recoverDelay = 0.5f;
    
    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float attackDamage = 1f;

    private float attackTimer;

    private Transform target;
    private NavMeshAgent navMeshAgent;
    private Rigidbody rigidBody;
    private bool isGrounded = true;
    private bool isLaunched;
    private float groundCheckDistance = 0.7f;
    private Vector3 groundNormal;
    private float launchTimer;

    public void Init(NavMeshAgent agent, Rigidbody rb)
    {
        navMeshAgent = agent;
        rigidBody = rb;
        navMeshAgent.enabled = true;

    }

    private void Update()
    {
        CheckGround();
        //todo dont let search happen every frame cuz it cost performance make it a scan every ~ second
        // or just a collider detection ontriggerenter
        FindTarget();
        if (isLaunched)
        {
            launchTimer -= Time.deltaTime;
            LaunchCheck();
        }
        HandleMovement();
        HandleAttack();
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
        if (navMeshAgent.enabled && !isLaunched && target != null)
        {
            navMeshAgent.SetDestination(target.position);
        }
    }

    private void FindTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);

        if (hits.Length > 0)
        {
            target = hits[0].transform;
        }
    }
    private void HandleAttack()
    {
        if(target == null)
            return;

        float distance = Vector3.Distance(
            transform.position,
            target.position
        );

        if(distance > attackRange)
            return;


        attackTimer -= Time.deltaTime;


        if(attackTimer <= 0)
        {
            attackTimer = attackCooldown;
            Attack();
        }
    }
    private void Attack()
    {
        IDamageable damageable = target.GetComponent<IDamageable>();

        if(damageable == null)
            return;


        DamageData damageData = new DamageData(
            attackDamage,
            DamageType.Physical,
            AttackPowerLevel.Normal
        );
        
        DamageResult result = damageable.TakeDamage(damageData);


        Debug.Log($"Enemy attack result: {result}");
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
