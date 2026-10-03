using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemySensor))]
[RequireComponent(typeof(EnemyMovement))]
[RequireComponent(typeof(EnemyAttack))]
[RequireComponent(typeof(EnemyStatus))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(HierarchicalStateMachine))]
public class Enemy : MonoBehaviour, ICharacter, IHasMovement, IHasAttack
{
    private EnemySensor sensor;
    private EnemyMovement enemyMovement;
    private EnemyAttack attack;
    private EnemyStatus enemyStatus;

    private HierarchicalStateMachine stateMachine;

    // todo future for stun etc
    public PlayerActionRestrictions Restrictions => PlayerActionRestrictions.None;

    #region Movement

    bool IHasMovement.IsGrounded => enemyMovement.IsGrounded;
    bool IHasMovement.IsMoving => sensor.HasTarget && sensor.DistanceToTarget > attack.AttackRange;
    public bool IsFalling => false;

    public void Jump()
    {
        return;
    }

    // only trigger move if theres target in range
    // change to check mode future or other stuff cuz if stunned dont want it to move etc
    void IHasMovement.Move()
    {
        if (sensor.Target != null)
        {
            enemyMovement.Move(sensor.Target.position);
        }
    }

    void IHasMovement.StopMove() => enemyMovement.StopMove();

    #endregion

    #region Attack

    bool IHasAttack.canAttack => sensor.HasTarget && sensor.DistanceToTarget <= attack.AttackRange;
    bool IHasAttack.isAttacking => attack.IsAttacking;
    bool IHasAttack.isComboQueued => false;
    void IHasAttack.HandleAtack() => attack.RequestAttack();

    #endregion

    private Rigidbody rigidBody;
    private NavMeshAgent navMeshAgent;
    private Animator animator;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        enemyStatus = GetComponent<EnemyStatus>();

        sensor = GetComponent<EnemySensor>();
        enemyMovement = GetComponent<EnemyMovement>();
        attack = GetComponent<EnemyAttack>();

        stateMachine = GetComponent<HierarchicalStateMachine>();

        // INIT
        EnemyStateFactory factory = new EnemyStateFactory(stateMachine, this);
        enemyMovement.Init(navMeshAgent, rigidBody, animator);
        attack.Init(animator);
        
        // this have to be last or else error
        stateMachine.Init(factory.Alive);

    }
}
