using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack", menuName = "AbilitySystem/Abilities/Attack")]
public class AttackAbilitySO : AbilityDefinitionSO
{
    [SerializeField] private float firstHitTime = 1f;
    [SerializeField] private float secondHitDelay = 0.5f;
    [SerializeField] private float endDelay = 0.5f;
    [SerializeField] private float comboWindow = 1.0f;
    [Header("Damage data")]
    [SerializeField] private float attackRange;
    [SerializeField] private DamageConfig damageConfig;
    [SerializeField] private LayerMask enemyLayerMask;


    
    private bool comboQueued;

    public override async Awaitable OnActivate(AbilitySystem ctx, CancellationToken token)
    {
        DebugDraw(ctx);

        ctx.Input.OnAttackPressed += QueueCombo;

        int combo = 1;
        while(combo <= 3)
        {
            // Do base attack + damage
            PerformDamageDetection(ctx);
            ctx.Animator.SetTrigger($"AttackTrigger{combo}");
            // Combo logic
            comboQueued = false;
            Debug.Log($"Combo : {combo}");
            Debug.Log("Combo window start");
            await Awaitable.WaitForSecondsAsync(comboWindow,token);
            if (!comboQueued)
            {
                return;
            }
            combo++;
            // while(true)
            // {
            //     string evt = 
            //         await ctx.WaitForAnimationEvent(token);
            //
            //
            //     switch(evt)
            //     {
            //         case "Hit":
            //             DealDamage(ctx);
            //             break;
            //
            //
            //         case "ComboOpen":
            //
            //             ctx.AddTag("State.ComboWindow");
            //
            //             break;
            //
            //
            //         case "ComboClose":
            //
            //             ctx.RemoveTag("State.ComboWindow");
            //
            //             break;
            //
            //
            //         case "Finished":
            //
            //             return;
            //     }
            
        }

    }

    private void QueueCombo()
    {
        comboQueued = true;
    }
    
    private void PerformDamageDetection(AbilitySystem ctx)
    {
        Vector3 attackPosition = ctx.Character.Transform.position;
        Collider[] hits = Physics.OverlapSphere(
            attackPosition,
            attackRange,
            enemyLayerMask
        );
    
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
                Debug.Log("attack hit");
            }
        }
    }

    //chatgpt circle line, ignore
    public void DebugDraw(AbilitySystem ctx)
    {
        Vector3 center = ctx.Character.Transform.position;

        int segments = 32;

        for (int i = 0; i < segments; i++)
        {
            float angle1 = i * Mathf.PI * 2 / segments;
            float angle2 = (i + 1) * Mathf.PI * 2 / segments;

            Vector3 point1 = center + new Vector3(
                Mathf.Cos(angle1),
                0,
                Mathf.Sin(angle1)
            ) * attackRange;

            Vector3 point2 = center + new Vector3(
                Mathf.Cos(angle2),
                0,
                Mathf.Sin(angle2)
            ) * attackRange;

            Debug.DrawLine(point1, point2, Color.red,2f);
        }
    }
    
    public override void OnEnd(AbilitySystem ctx)
    {
        Debug.Log("Atk ended");
        ctx.Input.OnAttackPressed -= QueueCombo;
    }

    public override void OnCancel(AbilitySystem ctx)
    {
        Debug.Log("Atk cancelled");
        ctx.Input.OnAttackPressed -= QueueCombo;
    }
}
