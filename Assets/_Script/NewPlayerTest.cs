using System;
using UnityEngine;
using System.Linq;

public class NewPlayerTest : MonoBehaviour
{
    public Collider Collider { get; private set; }
    public Rigidbody Rigidbody { get; private set; }
    public Animator Animator { get; private set; }
    public PlayerInputManager Input { get; private set; }
    public PlayerMovement Movement { get; private set; }
    public AbilitySystem AbilitySystem { get; private set; }
    
    private SkinnedMeshRenderer renderer;
    
    private Rigidbody[] ragdollRbs;
    private Collider[] ragdollCols;

    private void Awake()
    {
        Collider = GetComponent<Collider>();
        Rigidbody = GetComponent<Rigidbody>();
        Animator = GetComponent<Animator>();

        Input = FindFirstObjectByType<PlayerInputManager>();
        Movement = GetComponent<PlayerMovement>();
        AbilitySystem = GetComponent<AbilitySystem>();
        
        renderer = GetComponent<SkinnedMeshRenderer>();
        
        ragdollRbs = GetComponentsInChildren<Rigidbody>().Where(rb => rb.gameObject != gameObject).ToArray();
        ragdollCols = GetComponentsInChildren<Collider>() .Where(col => col.gameObject != gameObject) .ToArray();
    }

    private void OnEnable()
    {
        Input.OnAttackPressed += HandleAttack;
    }

    private void OnDisable()
    {
        Input.OnAttackPressed -= HandleAttack;
    }

    void HandleAttack()
    {
        AbilitySystem.TryActivateAbilityWithTag("Ability.Attack");
    }
}
