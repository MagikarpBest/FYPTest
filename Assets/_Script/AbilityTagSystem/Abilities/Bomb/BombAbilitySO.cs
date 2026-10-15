using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

[CreateAssetMenu(fileName = "Bomb", menuName = "AbilitySystem/Abilities/Bomb")]
public class BombAbilitySO : AbilityDefinitionSO
{
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionForce = 5f;
    [SerializeField] private float explodeDelay = 1.5f;
    [SerializeField] private float launchAngle = 45f;
    [SerializeField] private Bomb bombPrefab;
    [SerializeField] private float throwForce = 15f;
    [SerializeField] private float upwardForce = 5f;
    
    private PlayerInputManager input;
    private TaskCompletionSource<bool> abilityFinishedTcs;
    
    private Bomb activeBomb;
    private Transform bombHoldPoint;
    
    public async override Awaitable OnActivate(CancellationToken token)
    {
        if (!ctx.Character.TryGetComponent<PlayerInputManager>(out input)) return;
        if (bombPrefab == null) return;
        if (!ctx.Character.Sockets.TryGetSocket("Socket.Overhead", out bombHoldPoint)) return; //no socket set 
        
        abilityFinishedTcs = new TaskCompletionSource<bool>();
        token.Register(() => abilityFinishedTcs.TrySetCanceled()); //when token canceled set buildFinishedTcs to cancel also

        input.OnPrimaryActionPressed += ThrowBomb;
        input.OnSecondaryActionPressed += EndAbility;
        
        activeBomb = Instantiate(
            bombPrefab,
            bombHoldPoint.position,
            bombHoldPoint.rotation
        );
        
        activeBomb.Hold(bombHoldPoint);
        activeBomb.Init(explosionRadius, explosionForce, launchAngle);
        
        await abilityFinishedTcs.Task;
    }

    private void EndAbility() => abilityFinishedTcs.TrySetResult(true);

    private void ThrowBomb()
    {
        Vector3 throwDirection = Camera.main.transform.forward;

        float yaw = Mathf.Atan2( throwDirection.x, throwDirection.z) * Mathf.Rad2Deg;
        ctx.Character.Movement.RotateTowards(yaw);
        
        activeBomb.Throw(throwDirection, throwForce, upwardForce, explodeDelay);
        activeBomb = null;
        
        abilityFinishedTcs.TrySetResult(true);
    }

    public override void Cleanup()
    {
        input.OnPrimaryActionPressed -= ThrowBomb;
        input.OnSecondaryActionPressed -= EndAbility;
        
        if (activeBomb !=null)
        {
            Destroy(activeBomb.gameObject);
            activeBomb = null;
        }
    }
}
