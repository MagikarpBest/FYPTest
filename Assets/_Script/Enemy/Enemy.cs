using UnityEngine;
using UnityEngine.AI;
using System.Linq;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(EnemySensor))]
[RequireComponent(typeof(EnemyMovement))]
[RequireComponent(typeof(EnemyAttack))]
[RequireComponent(typeof(EnemyStatus))]
[RequireComponent(typeof(HierarchicalStateMachine))]
public class Enemy : MonoBehaviour, ICharacter
{
    public Collider Collider { get; private set; }
    public Rigidbody Rigidbody { get; private set; }
    public Animator Animator { get; private set; }
    public NavMeshAgent NavMeshAgent { get; private set; }
    
    public EnemySensor Sensor { get; private set; }
    public EnemyMovement Movement { get; private set; }
    public EnemyAttack AttackSystem { get; private set; }
    public EnemyStatus Status { get; private set; }

    private HierarchicalStateMachine stateMachine;
    
    private SkinnedMeshRenderer renderer;
    
    private Rigidbody[] ragdollRbs;
    private Collider[] ragdollCols;

    // todo future for stun etc
    public PlayerActionRestrictions Restrictions => PlayerActionRestrictions.None;
    
    private void Awake()
    {
        Collider = GetComponent<Collider>();    
        Rigidbody = GetComponent<Rigidbody>();
        NavMeshAgent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();
        Status = GetComponent<EnemyStatus>();

        Sensor = GetComponent<EnemySensor>();
        Movement = GetComponent<EnemyMovement>();
        AttackSystem = GetComponent<EnemyAttack>();

        stateMachine = GetComponent<HierarchicalStateMachine>();

        // INIT
        EnemyStateFactory factory = new EnemyStateFactory(stateMachine, this);
        Movement.Init(NavMeshAgent, Rigidbody, Animator);
        AttackSystem.Init(Animator);
        
        // this have to be last or else error
        stateMachine.Init(factory.Alive);

        renderer = GetComponent<SkinnedMeshRenderer>();
        
        ragdollRbs = GetComponentsInChildren<Rigidbody>().Where(rb => rb.gameObject != gameObject).ToArray();
        ragdollCols = GetComponentsInChildren<Collider>() .Where(col => col.gameObject != gameObject) .ToArray();
    }
    
    public void DisableRagdoll()
    {
        if (ragdollRbs == null || ragdollRbs.Length == 0 || ragdollCols == null || ragdollCols.Length == 0) return;
        
        Collider.enabled = true;
        Rigidbody.isKinematic = false;
        Animator.enabled = true;
        Sensor.enabled = true;
        Movement.enabled = true;
        AttackSystem.enabled = true;
        
        foreach (Collider col in ragdollCols) col.enabled = false;
        foreach (Rigidbody rb in ragdollRbs) rb.isKinematic = true;
    }

    public void EnableRagdoll()
    {
        if (ragdollRbs == null || ragdollRbs.Length == 0 || ragdollCols == null || ragdollCols.Length == 0) return;

        Collider.enabled = false;
        Rigidbody.isKinematic = true;
        Animator.enabled = false;
        Sensor.enabled = false;
        Movement.enabled = false;
        AttackSystem.enabled = false;
        
        foreach (Collider col in ragdollCols) col.enabled = true;
        foreach (Rigidbody rb in ragdollRbs) rb.isKinematic = false;
    }
}
