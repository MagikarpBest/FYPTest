using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;
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

    private AbilitySystem currentCtx;
    private bool canQueueCombo;
    private bool comboQueued;
    private TaskCompletionSource<bool> attackFinished;


    public override async Awaitable OnActivate(AbilitySystem ctx, CancellationToken token)
    {
        currentCtx = ctx;
        DebugDraw(currentCtx);

        currentCtx.Input.OnAttackPressed += QueueCombo;
        currentCtx.OnAnimationEvent += OnAnimationEvent;

        int comboIndex = 1;

        while (comboIndex <= 3)
        {
            // Combo logic
            comboQueued = false;
            attackFinished = new TaskCompletionSource<bool>();
            currentCtx.Animator.SetTrigger($"AttackTrigger{comboIndex}");

            // Wait until animation says combo finished
            bool continueCombo = await WaitForComboResult(token);

            if (!continueCombo)
            {
                break;
            }
            comboIndex++;
        }

    }

    private void OnAnimationEvent(string eventName)
    {
        switch (eventName)
        {
            case "Damage":
                PerformDamageDetection();
                break;

            case "ComboOpen":
                canQueueCombo = true;
                Debug.Log("Combo window Open");
                break;

            case "ComboClose":
                canQueueCombo = false;
                Debug.Log("Combo window Close");
                break;

            case "Finished":
                Debug.Log("Attack finish");
                attackFinished?.TrySetResult(comboQueued);

                break;
        }
    }

    private void QueueCombo()
    {
        if (!canQueueCombo)
            return;

        comboQueued = true;

        Debug.Log("Combo queued");
    }

    private void PerformDamageDetection()
    {
        Vector3 attackPosition = currentCtx.Character.Transform.position;
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

    private async Awaitable<bool> WaitForComboResult(CancellationToken token)
    {
        using (token.Register(() => { attackFinished.TrySetCanceled(); }))
        {
            return await attackFinished.Task;
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

            Debug.DrawLine(point1, point2, Color.red, 2f);
        }
    }

    public override void OnEnd(AbilitySystem ctx)
    {
        Debug.Log("Atk ended");
        ctx.Input.OnAttackPressed -= QueueCombo;
        ctx.OnAnimationEvent -= OnAnimationEvent;
    }

    public override void OnCancel(AbilitySystem ctx)
    {
        Debug.Log("Atk cancelled");
        ctx.Input.OnAttackPressed -= QueueCombo;
        ctx.OnAnimationEvent -= OnAnimationEvent;
    }
}
