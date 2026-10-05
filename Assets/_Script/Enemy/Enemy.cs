using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemySensor))]
[RequireComponent(typeof(EnemyMovement))]
[RequireComponent(typeof(EnemyAttack))]
[RequireComponent(typeof(EnemyStatus))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(HierarchicalStateMachine))]
public class Enemy : MonoBehaviour, ICharacter
{
    public GameObject Ragdoll { get; private set; }
    public EnemySensor Sensor { get; private set; }
    public EnemyMovement Movement { get; private set; }
    public EnemyAttack AttackSystem { get; private set; }
    public EnemyStatus Status { get; private set; }
    public EnemyDamageReceiver DamageReceiver{ get; private set; }

    private HierarchicalStateMachine stateMachine;

    // todo future for stun etc
    public PlayerActionRestrictions Restrictions => PlayerActionRestrictions.None;
    
    // only trigger move if theres target in range
    // change to check mode future or other stuff cuz if stunned dont want it to move etc
    // void IHasMovement.Move()
    // {
    //     if (sensor.Target != null)
    //     {
    //         enemyMovement.Move(sensor.Target.position);
    //     }
    // }
    
    private Rigidbody rigidBody;
    private NavMeshAgent navMeshAgent;
    private Animator animator;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        
        Status = GetComponent<EnemyStatus>();
        DamageReceiver = GetComponent<EnemyDamageReceiver>();

        Sensor = GetComponent<EnemySensor>();
        Movement = GetComponent<EnemyMovement>();
        AttackSystem = GetComponent<EnemyAttack>();

        stateMachine = GetComponent<HierarchicalStateMachine>();

        // INIT
        EnemyStateFactory factory = new EnemyStateFactory(stateMachine, this);
        Movement.Init(navMeshAgent, rigidBody, animator);
        AttackSystem.Init(animator);
        
        // this have to be last or else error
        stateMachine.Init(factory.Alive);

        Debug.Log("HELLO");
    }
}
