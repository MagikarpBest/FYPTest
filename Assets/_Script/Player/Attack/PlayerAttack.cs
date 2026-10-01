using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private bool isDebug;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange;
    [SerializeField] private int damage;
    [SerializeField] private LayerMask enemyLayerMask;

    public bool isAttacking { get; private set; }
    private bool canCombo;
    public bool comboQueued {get; private set;}
    private int comboIndex;
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
        if (!isAttacking)
        {
            StartAttack();
        }
        else if(canCombo)
        {
            comboQueued = true;
                Debug.Log("Combo Queued!");
        }
    }
    
    private void StartAttack()
    {
        animator.SetTrigger(AnimationParameter.AttackTrigger);
        isAttacking = true;
        comboQueued = false;
        canCombo = false;
        Collider[] hits = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayerMask);

        foreach (Collider hit in hits)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                DamageData damageData = new DamageData(
                    damage,
                    DamageType.Physical,
                    AttackPowerLevel.Normal
                );
                damageable.TakeDamage(damageData);
                //Debug.Log("attack hit");
            }
        }
        

        comboIndex++;
        if (comboIndex > 4)
        {
            comboIndex = 1;
            Debug.Log($"Combo over 3, reset to base");
        }
        Debug.Log($"Current combo{comboIndex}");
        EnableComboWindow();
        //Debug.Log(isAttacking);
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
        if(!comboQueued)
        {
            comboIndex = 0;
        }
        isAttacking = false;
        //Debug.Log("animation ended");
        //Debug.Log(isAttacking);
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
