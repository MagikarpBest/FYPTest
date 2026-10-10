using System;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(PlayerAttack))]
[RequireComponent(typeof(PlayerMovementRB))]
[RequireComponent(typeof(PlayerSkillController))]
[RequireComponent(typeof(HierarchicalStateMachine))]
[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(PlayerDamageReceiver))]
public class Player : MonoBehaviour, ICharacter
{
    [SerializeField] private Transform model;
    private Transform cameraTransform;
    
    public Collider Collider { get; private set; }
    public Rigidbody Rigidbody { get; private set; }
    public Animator Animator { get; private set; }
    
    public PlayerInputManager Input { get; private set; }
    public PlayerMovementRB Movement { get; private set; }
    public PlayerAttack AttackSystem { get; private set; }

    public PlayerSkillController SkillController { get; private set; } // Control skill logic
    public PlayerStats Stats { get; private set; } // Player data
    public PlayerDamageReceiver DamageReceiver { get; private set; } // 
    public PlayerModeController ModeController { get; private set; } // Control mode switches
    private HierarchicalStateMachine stateMachine;
    
    private SkinnedMeshRenderer renderer;
    
    private Rigidbody[] ragdollRbs;
    private Collider[] ragdollCols;
    
    public PlayerActionRestrictions Restrictions => ModeController.CurrentMode.Restrictions;
    
    private void Awake()
    {
        Collider = GetComponent<Collider>();
        Rigidbody = GetComponent<Rigidbody>();
        Animator = GetComponent<Animator>();
        cameraTransform = Camera.main.transform;

        Input = FindFirstObjectByType<PlayerInputManager>();
        Movement = GetComponent<PlayerMovementRB>();
        AttackSystem = GetComponent<PlayerAttack>();

        SkillController = GetComponent<PlayerSkillController>();
        Stats = GetComponent<PlayerStats>();
        DamageReceiver = GetComponent<PlayerDamageReceiver>();
        ModeController = GetComponent<PlayerModeController>();

        Movement.Init(Rigidbody, model, cameraTransform, Animator);
        AttackSystem.Init(Animator);
        
        stateMachine = GetComponent<HierarchicalStateMachine>();
        PlayerStateFactory factory = new PlayerStateFactory(stateMachine, this);
        
        stateMachine.Init(factory.Alive);
        
        renderer = GetComponent<SkinnedMeshRenderer>();
        
        ragdollRbs = GetComponentsInChildren<Rigidbody>().Where(rb => rb.gameObject != gameObject).ToArray();
        ragdollCols = GetComponentsInChildren<Collider>() .Where(col => col.gameObject != gameObject) .ToArray();
    }
    
    public void DisableRagdoll()
    {
        if (ragdollRbs == null || ragdollRbs.Length == 0 || ragdollCols == null || ragdollCols.Length == 0) return;
        
        Movement.StopMove();
        Animator.enabled = true;
        Rigidbody.isKinematic = false;
        Collider.enabled = true;
        Movement.enabled = true;
        AttackSystem.enabled = true;
        SkillController.enabled = true;
        ModeController.enabled = true;
        DamageReceiver.enabled = true;
        foreach (Collider col in ragdollCols) col.enabled = false;
        foreach (Rigidbody rb in ragdollRbs) rb.isKinematic = true;
    }

    public void EnableRagdoll()
    {
        if (ragdollRbs == null || ragdollRbs.Length == 0 || ragdollCols == null || ragdollCols.Length == 0) return;

        Animator.enabled = false;
        Rigidbody.isKinematic = true;
        Collider.enabled = false;
        Movement.enabled = false;
        AttackSystem.enabled = false;
        SkillController.enabled = false;
        ModeController.enabled = false;
        DamageReceiver.enabled = false;
        
        foreach (Collider col in ragdollCols) col.enabled = true;
        foreach (Rigidbody rb in ragdollRbs) rb.isKinematic = false;
    }
}
