using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour, IAttackSystem
{
    [SerializeField] private bool isDebug;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange;
    [SerializeField] private DamageConfig damageConfig;
    [SerializeField] private LayerMask enemyLayerMask;

    public bool IsAttacking { get; private set; }
    private bool canCombo;
    public bool IsComboQueued { get; private set; }
    private int comboIndex = 1;
    private float comboResetTimer;

    private Animator animator;

    public void Init(Animator playerAnimator)
    {
        animator = playerAnimator;
    }

    // private void Update()
    // {
    //     if (Mouse.current.leftButton.wasPressedThisFrame)
    //     {
    //         HandleAtack();
    //         if(animator == null)
    //         {
    //             Debug.Log("animator null");
    //         }
    //     }
    // }

    public void RequestAttack()
    {
        if (!IsAttacking)
        {
            StartAttack();
        }
        else if (canCombo)
        {
            IsComboQueued = true;
            //Debug.Log("Combo Queued!");
        }
    }

    private void StartAttack()
    {
        //Debug.Log($"START ATTACK - comboIndex={comboIndex}");

        if (comboIndex < 1 || comboIndex > 3) comboIndex = 1;

        switch (comboIndex)
        {
            case 1: animator.SetTrigger(AnimationParameter.AttackTrigger1); break;
            case 2: animator.SetTrigger(AnimationParameter.AttackTrigger2); break;
            case 3: animator.SetTrigger(AnimationParameter.AttackTrigger3); break;
        }
        
        IsAttacking = true;
        IsComboQueued = false;
        canCombo = false;

        PerformDamageDetection();

        if (comboIndex >= 3)
        {
            comboIndex = 1;
            //Debug.Log($"Combo over 3, reset to base");
        }
        else
        {
            comboIndex++;
        }
        //Debug.Log($"Current combo{comboIndex}");
        EnableComboWindow();
        //Debug.Log(isAttacking);
    }
    
    private void PerformDamageDetection()
    {
        Collider[] hits = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayerMask);

        foreach (Collider hit in hits)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                // Testing stuff so its easier to edit damage data than have to come to code to edit,
                DamageData damageData = new DamageData(
                    damageConfig.Damage,
                    damageConfig.DamageType,
                    damageConfig.PowerLevel
                );

                damageable.TakeDamage(damageData);
                //Debug.Log("attack hit");
            }
        }
    }

    private void EnableComboWindow()
    {
        canCombo = true;
    }

    private void DisableComboWindow()
    {
        canCombo = false;
    }


    // gotta change in future
    public void OnAttackAnimationEnd()
    {
        DisableComboWindow();
        animator.SetBool(AnimationParameter.IsComboQueued,IsComboQueued);

        if (!IsComboQueued)
        {
            comboIndex = 1;
        }
        IsAttacking = false;
        //Debug.Log($"Attack animation ended. Queued={comboQueued}");
        //Debug.Log($"After end: isAttacking={isAttacking}, comboIndex={comboIndex}");

    }

    private void OnDrawGizmos()
    {
        if (attackPoint == null || !isDebug)
        {
            return;
        }
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
