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
    
    private bool canQueueCombo;
    private bool comboQueued;
    private TaskCompletionSource<bool> attackFinished;
    private PlayerInputManager input;
    
    public override async Awaitable OnActivate(CancellationToken token)
    {
        // if (ctx.Character.Sockets.TryGetSocket("Socket.BombHold", out Transform bombHold))
        // {
        //     Instantiate(object, bombHold);
        // }
        
        DebugDraw();

        //if no input found just stop the ability
        if (!ctx.Character.TryGetComponent<PlayerInputManager>(out input))
        {
            Debug.Log("No input component attack cancelled");
            return;
        }
            
        input.OnPrimaryActionPressed += QueueCombo;
        ctx.OnAnimationEvent += OnAnimationEvent;

        int comboIndex = 1;

        while (comboIndex <= 3)
        {
            // Combo logic
            comboQueued = false;
            attackFinished = new TaskCompletionSource<bool>();
            ctx.Character.Animator.SetTrigger($"AttackTrigger{comboIndex}");
            //ctx.Character.Movement.OverrideMovement(ctx.Character.transform.forward * 11f, 0.2f);
            
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
                ctx.Character.Movement.OverrideMovement(ctx.Character.transform.forward * 11f, 0.1f);
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
        Vector3 attackPosition = ctx.Character.transform.position;
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
    public void DebugDraw()
    {
        Vector3 center = ctx.Character.transform.position;

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

    public override void OnEnd()
    {
        Debug.Log("Atk ended");
        input.OnPrimaryActionPressed -= QueueCombo;
        ctx.OnAnimationEvent -= OnAnimationEvent;
    }

    public override void OnCancel()
    {
        Debug.Log("Atk cancelled");
        input.OnPrimaryActionPressed -= QueueCombo;
        ctx.OnAnimationEvent -= OnAnimationEvent;
    }
}
