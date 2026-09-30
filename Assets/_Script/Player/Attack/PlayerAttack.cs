using System;
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

    public void HandleAtack()
    {
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
                Debug.Log("attack hit");
            }
        }
        animator.SetTrigger(AnimationParameter.AttackTrigger);
        isAttacking = true;
        Debug.Log(isAttacking);
    }
    
    // gotta change in future
    public void OnAttackAnimationEnd()
    {
        isAttacking = false;
        Debug.Log("animation ended");
        Debug.Log(isAttacking);
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
