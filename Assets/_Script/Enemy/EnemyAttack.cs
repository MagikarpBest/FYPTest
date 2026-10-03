using System;
using System.Collections;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private bool isDebug = false;
    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float attackDamage = 1f;
    [SerializeField] private float attackTakeTime = 1f;
    [SerializeField] private LayerMask playerLayerMask;


    private Animator animator;
    // attack
    private bool isAttacking;
    private float attackTimer;

    public float AttackRange => attackRange;
    public bool IsAttacking => isAttacking;

    public void Init(Animator anim)
    {
        animator = anim;
    }
    private void Update()
    {
        //Debug.Log(isAttacking);
    }

    // ATTACK
    public void RequestAttack()
    {
        attackTimer -= Time.deltaTime;


        if (attackTimer <= 0)
        {
            attackTimer = attackCooldown;
            isAttacking = true;
            StartCoroutine(Attack());
        }
    }

    // move this to animation event system alter
    private IEnumerator Attack()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, playerLayerMask);
        animator.SetTrigger(AnimationParameter.AttackTrigger1);
        foreach (var VARIABLE in hits)
        {
            IDamageable damageable = VARIABLE.GetComponent<IDamageable>();

            if (damageable == null)
                yield break;


            DamageData damageData = new DamageData(
                attackDamage,
                DamageType.Physical,
                AttackPowerLevel.Normal
            );

            DamageResult result = damageable.TakeDamage(damageData);

            yield return new WaitForSeconds(attackTakeTime);
            isAttacking = false;
            Debug.Log($"Enemy attack result: {result}");
        }
    }

    private void OnDrawGizmos()
    {
        if(isDebug)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }

    }

}
